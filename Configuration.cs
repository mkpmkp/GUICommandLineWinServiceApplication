using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Application1 {

	public class ElastoObject : DynamicObject {

		// Свойства класса
		Dictionary<string, object> Members = new Dictionary<string, object>();
		// Ссылка на родительский элемент дерева (не используется)
		public ElastoObject xParent;
		// Тип узла дерева (Не используется)
		public string xType;
		// Указатель на узел XML файла конфигурации, соответствующий данному свойству
		public XElement xXElement;
		// Флаг защиты от удаления (пока только для профиля)
		public bool Protected { get; }

		// Конструктор
		public ElastoObject(XElement n, bool isProtected = false) {
			xXElement = n;
			Protected = isProtected;
			foreach (var c in n.Elements()) {
				var cName = c.Name.ToString();
				var cType = c.Attribute("Type")?.Value ?? "String";
				if (Members.ContainsKey(cName)) throw new Exception(@"Ошибка конфигурационного файла Configuration.xml: дублируется элемент профиля @Name");
				Members[cName] = c.Value;
			}
		}

		// Возвращает все параметры профиля одним массивом
		public Dictionary<string, object> GetAllParameters() {
			return Members;
		}

		//
		public object Get(string Name, object Default = null) {
			if (Members.ContainsKey(Name)) return Members[Name];
			return Default;
		}

		//
		public bool Has(string Name) {
			return Members.ContainsKey(Name);
		}

		// Пытаемся присвоить значение
		public override bool TrySetMember(SetMemberBinder binder, object value) {
			if (value != null) {
				if (xXElement.Element(binder.Name) == null) {
					xXElement.Add(new XElement(binder.Name, value, new XAttribute(@"Type", @"String")));
				} else {
					xXElement.Element(binder.Name).Value = value.ToString();
					if (xXElement.Element(binder.Name).Attribute(@"Type") == null) {
						xXElement.Element(binder.Name).Add(new XAttribute("Type", "String"));
					} else {
						xXElement.Element(binder.Name).Attribute(@"Type").Value = "String";
					}
				}
				Members[binder.Name] = value;
			} else if (Members.ContainsKey(binder.Name)) {
				xXElement.Element(binder.Name).Remove();
				Members.Remove(binder.Name);
			}
			return true;
		}

		// Пытаемся прочесть значение
		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			if (Members.ContainsKey(binder.Name)) {
				result = Members[binder.Name].ToString();
				return true;
			}
			result = null;
			return true;
			//return base.TryGetMember(binder, out result);
		}

		// Пытаемся извлечь элемент (?)
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) {
			if (Members.ContainsKey(binder.Name)) {
				var d = Members[binder.Name] as Delegate;
				if (d != null) {
					result = d.DynamicInvoke(args);
					return true;
				}
			}
			return base.TryInvokeMember(binder, args, out result);
		}
	}

	//
	public class ConfigurationClass {
		// Имя файла конфигкрации
		private string ConfigurationFilePath;
		// Хранилище профилей
		private Dictionary<string, ElastoObject> Profiles = new Dictionary<string, ElastoObject>();
		// Собственно XML с конфигурацией
		private XDocument XML;
		// Имя текущего профиля
		private string CurrentProfileName;
		// Имя вторичного профиля
		private string SecondaryProfileName;
		// Указатель на текущий профиль
		public dynamic Current;
		// Указатель на дублирующий профиль
		public dynamic Secondary;

		// Конструктор
		public ConfigurationClass(string ConfigurationFile = null) {
			ConfigurationFilePath = ConfigurationFile ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Configuration.xml");
			Load();
		}

		// Загрузка конфигурации
		private void Load() {
			Profiles.Clear();
			try {
				XML = new XDocument();
				if (File.Exists(ConfigurationFilePath)) {
					// Open
					XML = XDocument.Load(ConfigurationFilePath);
				} else {
					// Create & Save
					XML = new XDocument(
						new XElement(@"Configuration",
							new XElement(@"Created", DateTime.Now),
							new XElement(@"CurrentProfile", @"Default"),
							new XElement(@"Profiles",
								new XElement(@"Profile", new XAttribute(@"Name", @"Default"))
							)
						)
					);
					Save();
				}

				// Создаем структуру конфигурации
				if (XML.Element(@"Configuration")?.Element(@"Profiles") == null) {
					XML.Element(@"Configuration").Add(new XElement(@"Profiles"));
				}
				foreach (var n in XML.Element(@"Configuration")?.Element(@"Profiles")?.Elements(@"Profile") ?? new XElement[0]) {
					var Name = n.Attribute(@"Name")?.Value;
					var Protected = (n.Attribute(@"Protected")?.Value ?? @"0") == "1" ? true : false;
					if (Name != null && !Profiles.ContainsKey(Name)) {
						Profiles[Name] = new ElastoObject(n, Protected) {
							xParent = null,
							xType = @"Profile"
						};
					} else {
						throw new Exception(@"Отсутствует или дублируется аттрибут профиля @Name");
					}
				}
				CurrentProfileName = CurrentProfileName ?? XML.Element(@"Configuration")?.Element(@"CurrentProfile")?.Value ?? GetFirstProfile() ?? @"Default";
				if (XML.Element(@"Configuration")?.Element(@"CurrentProfile") == null) {
					XML.Element(@"Configuration").Add(new XElement(@"CurrentProfile", CurrentProfileName));
				}
				if (!Profiles.ContainsKey(CurrentProfileName)) {
					var n = new XElement(@"Profile", new XAttribute(@"Name", CurrentProfileName));
					XML.Element(@"Configuration").Element(@"Profiles").Add(n);
					Profiles[CurrentProfileName] = new ElastoObject(n) {
						xParent = null,
						xType = @"Profile"
					};
				}
				Current = Profiles[CurrentProfileName];
				// Вторичный профиль
				SecondaryProfileName = CurrentProfileName;
				Secondary = Profiles[SecondaryProfileName];
			} catch (Exception Ex) {
				throw new Exception($"Ошибка открытия/создания конфигурационного файла Configuration.xml: {Ex.Message}");
			}
		}

		// Перезагружает конфигурацию с потерей всех изменений
		public void Reload() {
			Load();
		}

		// Сохраняет конфигурацию в файл
		public void Save() {
			try {
				XML.Save(ConfigurationFilePath);
			} catch (Exception Ex) {
				throw new Exception($"Ошибка записи конфигурационного файла Configuration.xml: {Ex.Message}");
			}
		}

		// Возвращает / устанавливает текущий профиль
		public string CurrentProfile(string ProfileName = null) {
			if (ProfileName == null) return CurrentProfileName;
			if (!Profiles.ContainsKey(ProfileName)) throw new Exception($"Профиль '{ProfileName}' не найден");
			CurrentProfileName = ProfileName;
			Current = Profiles[ProfileName];
			if (XML.Element("Configuration")?.Element("CurrentProfile").Value == null) {
				XML.Element("Configuration").Add(new XElement("CurrentProfile", ProfileName));
			} else {
				XML.Element("Configuration").Element("CurrentProfile").Value = ProfileName;
			}
			// Пишем физически в файл выбранный профиль
			try {
				var TmpXML = new XDocument();
				TmpXML = XDocument.Load(ConfigurationFilePath);
				TmpXML.Element("Configuration").Element("CurrentProfile").Value = ProfileName;
				TmpXML.Save(ConfigurationFilePath);
			} catch { }
			LOG.AppendLog(null, $"Профиль '{ProfileName}' установлен как текущий.", Color.Magenta);
			return ProfileName;
		}

		// Возвращает / устанавливает вторичный профиль
		public string SecondaryProfile(string ProfileName = null) {
			if (ProfileName == null) return SecondaryProfileName;
			if (Profiles.ContainsKey(ProfileName)) {
				SecondaryProfileName = ProfileName;
				Secondary = Profiles[ProfileName];
				return ProfileName;
			}
			throw new Exception($"Профиль '{ProfileName}' не найден");
		}

		// Возвращает массив с именами загруженных / созданных профилей
		public string[] GetProfiles() {
			return Profiles.Keys.ToArray();
		}

		// Создает новый профиль
		public void CreateProfile(string ProfileName) {
			if (Profiles.ContainsKey(ProfileName)) {
				throw new Exception($"Профиль с именем '{ProfileName}' уже существует");
			}
			var n = new XElement("Profile", new XAttribute("Name", ProfileName));
			XML.Element("Configuration").Element("Profiles").Add(n);
			dynamic d = new ElastoObject(n) {
				xParent = null,
				xType = "Profile"
			};
			Profiles[ProfileName] = d;
		}

		// Проверяет существование профиля
		public bool ProfileExists(string ProfileName) {
			return Profiles.ContainsKey(ProfileName);
		}

		// Возвращает имя первого профиля в хранилище
		public string GetFirstProfile() {
			if (Profiles.Count() > 0) return Profiles.First().Key;
			else return null;
		}

		// Удаляет профиль
		public void DeleteProfile(string ProfileName) {
			if (string.IsNullOrWhiteSpace(ProfileName)) {
				throw new Exception(@"Не указано имя профиля");
			}
			if (!Profiles.ContainsKey(ProfileName)) {
				throw new Exception($"Профиль '{ProfileName}' не найден");
			}
			if (ProfileName == CurrentProfileName) {
				throw new Exception(@"Нельзя удалить текущий профиль");
			}
			if (Profiles[ProfileName].Protected) {
				throw new Exception(@"Нельзя удалить защищенный профиль (@Protected=1)");
			}
			if (!string.IsNullOrWhiteSpace(ProfileName)) {
				XML.XPathSelectElements($"Configuration/Profiles/Profile[@Name='{ProfileName}']").Remove();
				Profiles.Remove(ProfileName);
			}
		}
	}
}

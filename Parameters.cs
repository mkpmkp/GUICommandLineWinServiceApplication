using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application1 {

	public class Parameters {

		private string RawParametersString;
		private string[] matches;

		public Dictionary<string, string> parameters { get; } = new Dictionary<string, string>() { };

		public Parameters(string ParametersString = null) {
			RawParametersString = ParametersString;
			var regex = new Regex(@"(?>^|\s+)(?>/|-{1,2})([a-z0-9]+)((?>=|:)?)", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
			matches = regex.Split(ParametersString?.Trim() ?? @"");

			// Если есть хоть 1 значение (первое пустое. остальные по 3 вхождения.)
			if (matches != null && matches.Count() > 3) {
				int count = (matches.Count() - 1) / 3;
				for (var i = 0; i < count; i++) {
					var Sign = matches[i * 3 + 2];
					if (Sign == @"=" || Sign == @":") {
						parameters[matches[i * 3 + 1].ToLower()] = matches[i * 3 + 3].Trim();
					} else {
						parameters[matches[i * 3 + 1].ToLower()] = @"True";
					}
				}
			}
		}

		//
		public string GetRawParametersString() {
			return RawParametersString;
		}

		public Dictionary<string, string> GetAllParameters() {
			return parameters;
		}

		//
		public void Report() {
			LOG.AppendLog(null, UTIL.ShowVar(matches));
			LOG.AppendLog(null, UTIL.ShowVar(parameters));
		}

		//
		public bool ContainsKey(string Name) {
			return parameters.ContainsKey(Name?.ToLower());
		}

		//
		public string Get(string Name, string Default = null) {
			return parameters.ContainsKey(Name.ToLower()) ? parameters[Name.ToLower()] : Default;
		}

		//
		public bool Add(string Name, string Value) {
			if (string.IsNullOrWhiteSpace(Name.Trim())) return false;
			parameters[Name.ToLower()] = Value;
			return true;
		}

		//
		public string Remove(string Name) {
			if (!parameters.ContainsKey(Name.ToLower())) return null;
			var ret = parameters[Name.ToLower()];
			parameters.Remove(Name.ToLower());
			return ret;
		}
	}

}

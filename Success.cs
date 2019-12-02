namespace Application1
{

	// Универсальный класс "Успех"
	public class Success : Event {

		// Конструктор
		public Success() : base(null, null, null, 0, null) {
		}

		// Конструктор
		public Success(string code = null, string message = null, object paramaters = null, int level = 0, string stacktrace = null) : base(code, message, paramaters, level, stacktrace) {
		}

	}
}

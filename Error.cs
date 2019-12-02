using System;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Application1
{

	public class ErrorParameters {
		public int ErrorLevel = 0;
		public string ThreadGUID;
		public string MessageGUID;
		public string OperationName;
		public string OperationHandler;
	}

	// Универсальный класс "Ошибка"
	public class Error : Event {

		// Конструктор
		public Error(object ErrorObject = null, string message = null, string stacktrace = null, ErrorParameters parameters = null) : base(stacktrace: @"") {
			var Lv = parameters?.ErrorLevel ?? 0;
			var ErrorCode = @"INT000000";
			if (ErrorObject == null) {
				ErrorCode = @"#INT000000";
			} else if (ErrorObject.GetType() == typeof(string)) {
				ErrorCode = (string)ErrorObject ?? "#INT000000";
			} else if (ErrorObject.GetType() == typeof(int)) {
				ErrorCode = $"#INT{(int)ErrorObject:D10}";
			} else if (ErrorObject.GetType() == typeof(CryptographicException)) {
				var ExObject = ErrorObject as CryptographicException;
				ErrorCode = @"#CRP000000";
				Lv = 2;
				if (message == null) message = $"Ex (CryptographicException): {ExObject.Message}";
				if (stacktrace == null) stacktrace = $"{ExObject.StackTrace}\r\nInnerException:\r\nMessage: {ExObject.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.StackTrace}\r\nInnerInnerException:\r\nMessage:{ExObject.InnerException?.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.InnerException?.StackTrace}";
			} else if (ErrorObject.GetType() == typeof(SqlException)) {
				var ExObject = ErrorObject as SqlException;
				ErrorCode = $"#SQL{ExObject.Number:D10}";
				Lv = 1;
				if (message == null) message = $"Ex (SqlException): {ExObject.Message}, StordeProcedure [{ExObject.Procedure}]";
				if (stacktrace == null) stacktrace = $"{ExObject.StackTrace}\r\nInnerException:\r\nMessage: {ExObject.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.StackTrace}\r\nInnerInnerException:\r\nMessage:{ExObject.InnerException?.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.InnerException?.StackTrace}";
			} else if (ErrorObject.GetType() == typeof(System.ServiceModel.Security.MessageSecurityException)) {
				var ExObject = ErrorObject as System.ServiceModel.Security.MessageSecurityException;
				ErrorCode = @"#NWS000000";
				Lv = 2;
				if (message == null) message = $"Ex: {ExObject.Message}";
				if (stacktrace == null) stacktrace = $"{ExObject.StackTrace}\r\nInnerException:\r\nMessage: {ExObject.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.StackTrace}\r\nInnerInnerException:\r\nMessage:{ExObject.InnerException?.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.InnerException?.StackTrace}";
			} else if (ErrorObject is Exception ExObject) {
				ErrorCode = @"#EXP000000";
				Lv = 2;
				if (message == null) {
					message = $"Ex ({ErrorObject.GetType()}): {ExObject.Message}";
					// Добавил InnerException
					if (ExObject.InnerException != null) {
						message += $"; InnerEx ({ExObject.InnerException.GetType()}): {ExObject.InnerException.Message}";
					}
				}
				if (stacktrace == null) stacktrace = $"{ExObject.StackTrace}\r\nInnerException:\r\nMessage: {ExObject.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.StackTrace}\r\nInnerInnerException:\r\nMessage:{ExObject.InnerException?.InnerException?.Message}\r\nStackTrace:\r\n{ExObject.InnerException?.InnerException?.StackTrace}";
			} else {
				ErrorCode = @"#INT000000";
			}

			// Заполняем свойства события
			SetCode(ErrorCode);
			SetMessage(message);
			SetParameters(parameters);
			SetLevel(Lv);
			SetStackTrace(stacktrace ?? Environment.StackTrace);
		}

		//
		public string GetMessage(bool WithStackTrace = false) {
			return $"({Code}) {Message}{(WithStackTrace ? "\n\rStackTrace:\n\r" + StackTrace : null)}";
		}

	}
}

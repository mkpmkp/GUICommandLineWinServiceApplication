using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application1 {

	public class Event {

		public string Code { get; private set; } = null;
		public string Message { get; private set; } = null;
		public object Parameters { get; private set; } = null;
		public int Level { get; private set; } = 0;
		public string Caller { get; private set; } = @"N/A";
		public string CallerChain { get; private set; } = @"N/A";
		public string StackTrace { get; private set; } = null;

		//
		public Event(string code = null, string message = null, object parameters = null, int level = 0, string stacktrace = null) {
			Code = code;
			Message = message;
			Parameters = parameters;
			Level = Math.Max(0, level);
			Caller = GetCurrentCaller(3);
			CallerChain = GetCurrentCallerChain();
			StackTrace = stacktrace ?? Environment.StackTrace;
		}

		//
		public string GetCode() {
			return Code;
		}

		//
		public void SetCode(string code = null) {
			Code = code;
		}

		//
		public string GetMessage() {
			if (Code == null) {
				return Message;
			} else {
				return $"({Code}) {Message}";
			}
		}

		//
		public void SetMessage(string message = null) {
			Message = message;
		}

		//
		public object GetParameters() {
			return Parameters;
		}

		//
		public void SetParameters(object parameters = null) {
			Parameters = parameters;
		}

		//
		public int ? GetLevel() {
			return Level;
		}

		//
		public void SetLevel(int level = 0) {
			Level = Math.Max(0, level);
		}

		//
		public string GetCaller() {
			return Caller;
		}

		//
		public void SetStackTrace(string stacktrace) {
			StackTrace = stacktrace;
		}

		//
		public string GetStackTrace() {
			return StackTrace;
		}

		//
		public static string GetCurrentCaller(int frame = 1) {
			try {
				if (frame < 0) return @"ERR";
				var Stack = new StackTrace();
				if (Stack.FrameCount < frame + 1) return @"N/A";
				var Method = Stack.GetFrame(frame).GetMethod();
				string[] ClassName = (Method.DeclaringType.ToString()).Split('.');
				return $"{ClassName.Last()}:{Method.Name}";
			} catch {
				return @"N/A";
			}
		}

		//
		public static string GetCurrentCallerChain() {
			try {
				var Stack = new StackTrace();
				var s = new List<string>();
				for (int i = 1; i < Stack.FrameCount; i++) {
					var f = Stack.GetFrame(i);
					var m = f.GetMethod();
					string[] cn = (m.DeclaringType.ToString()).Split('.');
					s.Add($"{cn.Last()}:{m.Name}");
				}
				return string.Join(@"|", s);
			} catch {
				return @"N/A";
			}
		}

	}

}

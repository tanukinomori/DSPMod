using BepInEx.Logging;
using System.Reflection;

namespace Tanukinomori
{
	public static class LogManager
	{
		public static ManualLogSource Logger { private get; set; }

		public static void LogInfo(object data) =>
			Logger.LogInfo(data);

		public static void LogInfo(MethodBase method) =>
			Logger.LogInfo(method.DeclaringType.Name + "." + method.Name);

		public static void LogError(object data) =>
			Logger.LogError(data);

		public static void LogError(MethodBase method, object data) =>
			Logger.LogError(method.DeclaringType.Name + "." + method.Name + ": " + data);
	}
}

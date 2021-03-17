using BepInEx.Logging;

namespace Tanukinomori
{
	public static class LogManager
	{
		public static ManualLogSource Logger { private get; set; }

		public static void LogInfo(object data) =>
			Logger.LogInfo(data);
	}
}

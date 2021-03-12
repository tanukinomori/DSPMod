using BepInEx.Configuration;

namespace Tanukinomori
{
	public class CruiseAssistConfigManager : ConfigManager
	{
		public CruiseAssistConfigManager(ConfigFile Config) : base(Config) {
		}

		override protected void ConfigReloadImplements(Step step) {
			bool saveFlag = false;
			if (step == Step.AWAKE) {
			} else if (step == Step.GAME_MAIN_BEGIN) {
			}
			if (saveFlag) {
				Save(false);
			}
			LogManager.Logger.LogInfo("config reloaded.");
		}
	}
}

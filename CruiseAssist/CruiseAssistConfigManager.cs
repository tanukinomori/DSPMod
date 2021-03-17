using BepInEx.Configuration;

namespace Tanukinomori
{
	public class CruiseAssistConfigManager : ConfigManager
	{
		public CruiseAssistConfigManager(ConfigFile Config) : base(Config)
		{
		}

		override protected void CheckConfigImplements(Step step)
		{
			bool saveFlag = false;
			if (step == Step.AWAKE)
			{
				var modVersion = Bind<string>("Base", "ModVersion", CruiseAssist.ModVersion, "Don't change.");
				modVersion.Value = CruiseAssist.ModVersion;
				CruiseAssistUI.Rect.x = Bind<float>("State", "InfoWindowLeft", 100.0f).Value;
				CruiseAssistUI.Rect.y = Bind<float>("State", "InfoWindowTop", 100.0f).Value;
				saveFlag = true;
			}
			else if (step == Step.GAME_MAIN_BEGIN)
			{
			}
			else if (step == Step.STATE)
			{
				var leftEntry = ConfigManager.GetEntry<float>("State", "InfoWindowLeft");
				if (leftEntry.Value != CruiseAssistUI.Rect.x)
				{
					leftEntry.Value = CruiseAssistUI.Rect.x;
					saveFlag = true;
				}
				var topEntry = ConfigManager.GetEntry<float>("State", "InfoWindowTop");
				if (topEntry.Value != CruiseAssistUI.Rect.y)
				{
					topEntry.Value = CruiseAssistUI.Rect.y;
					saveFlag = true;
				}
			}
			if (saveFlag)
			{
				Save(false);
			}
			if (step == Step.AWAKE)
			{
				LogManager.LogInfo("config reloaded.");
			}
		}
	}
}

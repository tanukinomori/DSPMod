using BepInEx.Configuration;

namespace Tanukinomori
{
	public class CruiseAssistConfigManager : ConfigManager
	{
		public CruiseAssistConfigManager(ConfigFile Config) : base(Config)
		{
		}

		protected override void CheckConfigImplements(Step step)
		{
			bool saveFlag = false;
			if (step == Step.AWAKE)
			{
				var modVersion = Bind<string>("Base", "ModVersion", CruiseAssist.ModVersion, "Don't change.");
				modVersion.Value = CruiseAssist.ModVersion;
				CruiseAssistMainUI.Rect.x = (float)Bind<int>("State", "MainWindowLeft", 100).Value;
				CruiseAssistMainUI.Rect.y = (float)Bind<int>("State", "MainWindowTop", 100).Value;
				CruiseAssistStarListUI.Rect.x = (float)Bind<int>("State", "StarListWindowLeft", 100).Value;
				CruiseAssistStarListUI.Rect.y = (float)Bind<int>("State", "StarListWindowTop", 100).Value;
				var orphanedEntries = ConfigManager.GetOrphanedEntries();
				string s;
				float f;
				if (orphanedEntries.TryGetValue(new ConfigDefinition("State", "InfoWindowLeft"), out s) && float.TryParse(s, out f))
				{
					CruiseAssistMainUI.Rect.x = f;
				}
				if (orphanedEntries.TryGetValue(new ConfigDefinition("State", "InfoWindowTop"), out s) && float.TryParse(s, out f))
				{
					CruiseAssistMainUI.Rect.y = f;
				}
				orphanedEntries.Clear();
				saveFlag = true;
			}
			else if (step == Step.GAME_MAIN_BEGIN)
			{
			}
			else if (step == Step.STATE)
			{
				ConfigEntry<int> entry;
				entry = ConfigManager.GetEntry<int>("State", "MainWindowLeft");
				if (entry.Value != (int)CruiseAssistMainUI.Rect.x)
				{
					entry.Value = (int)CruiseAssistMainUI.Rect.x;
					saveFlag = true;
				}
				entry = ConfigManager.GetEntry<int>("State", "MainWindowTop");
				if (entry.Value != (int)CruiseAssistMainUI.Rect.y)
				{
					entry.Value = (int)CruiseAssistMainUI.Rect.y;
					saveFlag = true;
				}
				entry = ConfigManager.GetEntry<int>("State", "StarListWindowLeft");
				if (entry.Value != (int)CruiseAssistStarListUI.Rect.x)
				{
					entry.Value = (int)CruiseAssistStarListUI.Rect.x;
					saveFlag = true;
				}
				entry = ConfigManager.GetEntry<int>("State", "StarListWindowTop");
				if (entry.Value != (int)CruiseAssistStarListUI.Rect.y)
				{
					entry.Value = (int)CruiseAssistStarListUI.Rect.y;
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

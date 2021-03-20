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
				CruiseAssistDebugUI.Show = Bind<bool>("Debug", "DebugWindowShow", false).Value;
				CruiseAssist.Enable = Bind<bool>("Setting", "Enable", true).Value;
				var viewModeStr = Bind<string>("Setting", "MainWindowViewMode", CruiseAssistMainUIViewMode.FULL.ToString()).Value;
				EnumUtils.TryParse<CruiseAssistMainUIViewMode>(viewModeStr, out CruiseAssistMainUI.ViewMode);
				CruiseAssistMainUI.Rect.x = (float)Bind<int>("State", "MainWindowLeft", 100).Value;
				CruiseAssistMainUI.Rect.y = (float)Bind<int>("State", "MainWindowTop", 100).Value;
				CruiseAssistStarListUI.Rect.x = (float)Bind<int>("State", "StarListWindowLeft", 100).Value;
				CruiseAssistStarListUI.Rect.y = (float)Bind<int>("State", "StarListWindowTop", 100).Value;
				CruiseAssistDebugUI.Rect.x = (float)Bind<int>("State", "DebugWindowLeft", 100).Value;
				CruiseAssistDebugUI.Rect.y = (float)Bind<int>("State", "DebugWindowTop", 100).Value;
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
				ConfigEntry<bool> boolEntry;
				ConfigEntry<string> strEntry;
				ConfigEntry<int> intEntry;
				boolEntry = ConfigManager.GetEntry<bool>("Setting", "Enable");
				if (boolEntry.Value != CruiseAssist.Enable)
				{
					boolEntry.Value = CruiseAssist.Enable;
					saveFlag = true;
				}
				strEntry = ConfigManager.GetEntry<string>("Setting", "MainWindowViewMode");
				if (strEntry.Value != CruiseAssistMainUI.ViewMode.ToString())
				{
					strEntry.Value = CruiseAssistMainUI.ViewMode.ToString();
					saveFlag = true;
				}
				intEntry = ConfigManager.GetEntry<int>("State", "MainWindowLeft");
				if (intEntry.Value != (int)CruiseAssistMainUI.Rect.x)
				{
					intEntry.Value = (int)CruiseAssistMainUI.Rect.x;
					saveFlag = true;
				}
				intEntry = ConfigManager.GetEntry<int>("State", "MainWindowTop");
				if (intEntry.Value != (int)CruiseAssistMainUI.Rect.y)
				{
					intEntry.Value = (int)CruiseAssistMainUI.Rect.y;
					saveFlag = true;
				}
				intEntry = ConfigManager.GetEntry<int>("State", "StarListWindowLeft");
				if (intEntry.Value != (int)CruiseAssistStarListUI.Rect.x)
				{
					intEntry.Value = (int)CruiseAssistStarListUI.Rect.x;
					saveFlag = true;
				}
				intEntry = ConfigManager.GetEntry<int>("State", "StarListWindowTop");
				if (intEntry.Value != (int)CruiseAssistStarListUI.Rect.y)
				{
					intEntry.Value = (int)CruiseAssistStarListUI.Rect.y;
					saveFlag = true;
				}
				intEntry = ConfigManager.GetEntry<int>("State", "DebugWindowLeft");
				if (intEntry.Value != (int)CruiseAssistDebugUI.Rect.x)
				{
					intEntry.Value = (int)CruiseAssistDebugUI.Rect.x;
					saveFlag = true;
				}
				intEntry = ConfigManager.GetEntry<int>("State", "DebugWindowTop");
				if (intEntry.Value != (int)CruiseAssistDebugUI.Rect.y)
				{
					intEntry.Value = (int)CruiseAssistDebugUI.Rect.y;
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

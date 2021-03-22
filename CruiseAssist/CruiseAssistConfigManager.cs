using BepInEx.Configuration;
using System.Linq;
using Tanukinomori.commons;

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
				var orphanedEntries = ConfigManager.GetOrphanedEntries();
				Migration("State", "MainWindow0Left", 100, "State", "InfoWindowLeft", orphanedEntries);
				Migration("State", "MainWindow0Top", 100, "State", "InfoWindowTop", orphanedEntries);
				Migration("State", "MainWindow0Left", 100, "State", "MainWindowLeft", orphanedEntries);
				Migration("State", "MainWindow0Top", 100, "State", "MainWindowTop", orphanedEntries);
				Migration("State", "StarListWindow0Left", 100, "State", "StarListWindowLeft", orphanedEntries);
				Migration("State", "StarListWindow0Top", 100, "State", "StarListWindowTop", orphanedEntries);
				saveFlag = true;
			}
			if (step == Step.AWAKE || step == Step.GAME_MAIN_BEGIN)
			{
				CruiseAssistDebugUI.Show = Bind("Debug", "DebugWindowShow", false).Value;
				CruiseAssist.Enable = Bind("Setting", "Enable", true).Value;
				var viewModeStr = Bind("Setting", "MainWindowViewMode", CruiseAssistMainUIViewMode.FULL.ToString()).Value;
				EnumUtils.TryParse<CruiseAssistMainUIViewMode>(viewModeStr, out CruiseAssistMainUI.ViewMode);
				for (int i = 0; i < 2; ++i)
				{
					CruiseAssistMainUI.Rect[i].x = (float)Bind("State", $"MainWindow{i}Left", 100).Value;
					CruiseAssistMainUI.Rect[i].y = (float)Bind("State", $"MainWindow{i}Top", 100).Value;
					CruiseAssistStarListUI.Rect[i].x = (float)Bind("State", $"StarListWindow{i}Left", 100).Value;
					CruiseAssistStarListUI.Rect[i].y = (float)Bind("State", $"StarListWindow{i}Top", 100).Value;
				}
				CruiseAssistStarListUI.ListSelected = Bind("State", "StarListWindowListSelected", 0).Value;
				CruiseAssistDebugUI.Rect.x = (float)Bind("State", "DebugWindowLeft", 100).Value;
				CruiseAssistDebugUI.Rect.y = (float)Bind("State", "DebugWindowTop", 100).Value;
				if (GameMain.galaxy != null)
				{
					CruiseAssist.History = ListUtils.Parse(Bind("Save", $"History_{GameMain.galaxy.seed}", "").Value);
				}
			}
			else if (step == Step.STATE)
			{
				ConfigEntry<bool> boolEntry;
				ConfigEntry<string> strEntry;
				ConfigEntry<int> intEntry;
				string strValue;
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
				for (int i = 0; i < 2; ++i)
				{
					intEntry = ConfigManager.GetEntry<int>("State", $"MainWindow{i}Left");
					if (intEntry.Value != (int)CruiseAssistMainUI.Rect[i].x)
					{
						intEntry.Value = (int)CruiseAssistMainUI.Rect[i].x;
						saveFlag = true;
					}
					intEntry = ConfigManager.GetEntry<int>("State", $"MainWindow{i}Top");
					if (intEntry.Value != (int)CruiseAssistMainUI.Rect[i].y)
					{
						intEntry.Value = (int)CruiseAssistMainUI.Rect[i].y;
						saveFlag = true;
					}
					intEntry = ConfigManager.GetEntry<int>("State", $"StarListWindow{i}Left");
					if (intEntry.Value != (int)CruiseAssistStarListUI.Rect[i].x)
					{
						intEntry.Value = (int)CruiseAssistStarListUI.Rect[i].x;
						saveFlag = true;
					}
					intEntry = ConfigManager.GetEntry<int>("State", $"StarListWindow{i}Top");
					if (intEntry.Value != (int)CruiseAssistStarListUI.Rect[i].y)
					{
						intEntry.Value = (int)CruiseAssistStarListUI.Rect[i].y;
						saveFlag = true;
					}
				}
				intEntry = ConfigManager.GetEntry<int>("State", "StarListWindowListSelected");
				if (intEntry.Value != CruiseAssistStarListUI.ListSelected)
				{
					intEntry.Value = CruiseAssistStarListUI.ListSelected;
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
				strEntry = ConfigManager.GetEntry<string>("Save", $"History_{GameMain.galaxy.seed}");
				strValue = ListUtils.ToString(CruiseAssist.History);
				if (strEntry.Value != strValue)
				{
					strEntry.Value = strValue;
					saveFlag = true;
				}
			}
			if (saveFlag)
			{
				Save(false);
			}
			if (step == Step.AWAKE || step == Step.GAME_MAIN_BEGIN)
			{
				LogManager.LogInfo("config reloaded.");
			}
		}
	}
}

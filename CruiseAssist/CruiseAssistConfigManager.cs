﻿using BepInEx.Configuration;
using System.Linq;
using Tanukinomori.commons;
using Tanukinomori.UI;

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

				Migration("State", "MainWindow0Left", 100, "State", "InfoWindowLeft");
				Migration("State", "MainWindow0Top", 100, "State", "InfoWindowTop");
				Migration("State", "MainWindow0Left", 100, "State", "MainWindowLeft");
				Migration("State", "MainWindow0Top", 100, "State", "MainWindowTop");
				Migration("State", "StarListWindow0Left", 100, "State", "StarListWindowLeft");
				Migration("State", "StarListWindow0Top", 100, "State", "StarListWindowTop");

				saveFlag = true;
			}
			if (step == Step.AWAKE || step == Step.GAME_MAIN_BEGIN)
			{
				CruiseAssistDebugUI.Show = Bind("Debug", "DebugWindowShow", false).Value;

				CruiseAssist.Enable = Bind("Setting", "Enable", true).Value;

				CruiseAssistMainUI.Scale = (float)Bind("Setting", "UIScale", 200).Value;

				var viewModeStr = Bind("Setting", "MainWindowViewMode", CruiseAssistMainUIViewMode.FULL.ToString()).Value;
				EnumUtils.TryParse<CruiseAssistMainUIViewMode>(viewModeStr, out CruiseAssistMainUI.ViewMode);

				for (int i = 0; i < 2; ++i)
				{
					CruiseAssistMainUI.Rect[i].x = (float)Bind("State", $"MainWindow{i}Left", 100).Value;
					CruiseAssistMainUI.Rect[i].y = (float)Bind("State", $"MainWindow{i}Top", 100).Value;
					CruiseAssistStarListUI.Rect[i].x = (float)Bind("State", $"StarListWindow{i}Left", 100).Value;
					CruiseAssistStarListUI.Rect[i].y = (float)Bind("State", $"StarListWindow{i}Top", 100).Value;
					CruiseAssistConfigUI.Rect[i].x = (float)Bind("State", $"ConfigWindow{i}Left", 100).Value;
					CruiseAssistConfigUI.Rect[i].y = (float)Bind("State", $"ConfigWindow{i}Top", 100).Value;
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
				saveFlag |= ConfigManager.UpdateEntry("Setting", "Enable", CruiseAssist.Enable);

				saveFlag |= ConfigManager.UpdateEntry("Setting", "UIScale", (int)CruiseAssistMainUI.Scale);

				saveFlag |= ConfigManager.UpdateEntry("Setting", "MainWindowViewMode", CruiseAssistMainUI.ViewMode.ToString());

				for (int i = 0; i < 2; ++i)
				{
					saveFlag |= ConfigManager.UpdateEntry("State", $"MainWindow{i}Left", (int)CruiseAssistMainUI.Rect[i].x);
					saveFlag |= ConfigManager.UpdateEntry("State", $"MainWindow{i}Top", (int)CruiseAssistMainUI.Rect[i].y);
					saveFlag |= ConfigManager.UpdateEntry("State", $"StarListWindow{i}Left", (int)CruiseAssistStarListUI.Rect[i].x);
					saveFlag |= ConfigManager.UpdateEntry("State", $"StarListWindow{i}Top", (int)CruiseAssistStarListUI.Rect[i].y);
					saveFlag |= ConfigManager.UpdateEntry("State", $"ConfigWindow{i}Left", (int)CruiseAssistConfigUI.Rect[i].x);
					saveFlag |= ConfigManager.UpdateEntry("State", $"ConfigWindow{i}Top", (int)CruiseAssistConfigUI.Rect[i].y);
				}

				saveFlag |= ConfigManager.UpdateEntry("State", "StarListWindowListSelected", CruiseAssistStarListUI.ListSelected);

				saveFlag |= ConfigManager.UpdateEntry("State", "DebugWindowLeft", (int)CruiseAssistDebugUI.Rect.x);
				saveFlag |= ConfigManager.UpdateEntry("State", "DebugWindowTop", (int)CruiseAssistDebugUI.Rect.y);

				saveFlag |= ConfigManager.UpdateEntry("Save", $"History_{GameMain.galaxy.seed}", ListUtils.ToString(CruiseAssist.History));
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

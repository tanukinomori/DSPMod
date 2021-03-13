using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Tanukinomori
{
	public class ChangeInventorySizeConfigManager : ConfigManager
	{
		public ChangeInventorySizeConfigManager(ConfigFile Config) : base(Config)
		{
		}

		override protected void ConfigReloadImplements(Step step)
		{
			bool saveFlag = false;
			if (step == Step.AWAKE)
			{
				Bind<string>("Base", "ModVersion", ChangeInventorySize.ModVersion, "Don't change.");
				Bind<int>("Setting", "InventorySize", 160);
				saveFlag = true;
			}
			else if (step == Step.GAME_MAIN_BEGIN)
			{
				ConfigManager.Config.Reload();
			}
			if (saveFlag)
			{
				Save(false);
			}
			LogManager.Logger.LogInfo("config reloaded.");
		}
	}
}

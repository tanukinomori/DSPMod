using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace TankManualInOutAmountMultiply
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class TankManualInOutAmountMultiply : BaseUnityPlugin
	{
		public const string ModGuid = "jp.co.tanukinomori.dsp.tankmanualinoutamountmultiply";
		public const string ModName = "TankManualInOutAmountMultiply";
		public const string ModVersion = "0.0.1";

		new internal static ManualLogSource Logger;
		new internal static ConfigFile Config;

		public static int ConfigMultiValue;
		public static int TankManualInOutMaxAmount;

		public void Awake() {
			TankManualInOutAmountMultiply.Logger = base.Logger;
			TankManualInOutAmountMultiply.Config = base.Config;
			TankManualInOutAmountMultiply.Config.SaveOnConfigSet = false;
			OnConfigReload(true);
			Config.ConfigReloaded += OnConfigReload;
			var harmony = new Harmony("jp.co.tanukinomori.dsp.tankmanualinoutamountmultiply.patch");
			harmony.PatchAll(typeof(Patch_GameMain_Begin));
			harmony.PatchAll(typeof(Patch_UITankWindow_OnUpdate));
		}

		public static void OnConfigReload(object sender, EventArgs e) {
			OnConfigReload();
		}

		public static void OnConfigReload(bool saveFlag = false) {
			Config.Bind<string>("Base", "ModVersion", ModVersion, "Don't change.");
			ConfigMultiValue = Config.Bind<int>("Base", "MultiplyValue", 5).Value;
			TankManualInOutMaxAmount = 2 * ConfigMultiValue;
			if (saveFlag) {
				var OrphanedEntries = (Dictionary<ConfigDefinition, string>)typeof(ConfigFile).GetProperty("OrphanedEntries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Config, null);
				OrphanedEntries.Clear();
				Config.Save();
				TankManualInOutAmountMultiply.Logger.LogInfo("save config.");
			}
			TankManualInOutAmountMultiply.Logger.LogInfo("config reloaded.");
		}
	}
}

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace FoundationMask
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class FoundationMask : BaseUnityPlugin
	{
		public const string ModGuid = "jp.co.tanukinomori.dsp.foundationmask";
		public const string ModName = "FoundationMask";
		public const string ModVersion = "0.0.4";

		new internal static ManualLogSource Logger;
		new internal static ConfigFile Config;

		public static int ConfigMaskSizeX;
		public static int ConfigMaskSizeY;
		public static string[] ConfigMask = null;

		public void Awake() {
			FoundationMask.Logger = base.Logger;
			FoundationMask.Config = base.Config;
			FoundationMask.Config.SaveOnConfigSet = false;
			OnConfigReload(true);
			Config.ConfigReloaded += OnConfigReload;
			var harmony = new Harmony("jp.co.tanukinomori.dsp.foundationmask.patch");
			harmony.PatchAll(typeof(Patch_GameMain_Begin));
			harmony.PatchAll(typeof(Patch_PlanetGrid_ReformSnapTo));
		}

		public static void OnConfigReload(object sender, EventArgs e) {
			OnConfigReload();
		}

		public static void OnConfigReload(bool saveFlag = false) {
			var configModVer = Config.Bind<string>("Base", "ModVersion", "0.0.0", "Don't change.").Value;
			if (configModVer == "0.0.0") {
				FoundationMask.Logger.LogInfo("clear config.");
				Config.Clear();
				saveFlag = true;
				Config.Bind<string>("Base", "ModVersion", ModVersion, "Don't change.");
				Config.Bind<int>("Mask", "PatternSizeX", 3);
				Config.Bind<int>("Mask", "PatternSizeY", 3);
				Config.Bind<string>("Mask", "Pattern0", "100");
				Config.Bind<string>("Mask", "Pattern1", "000");
				Config.Bind<string>("Mask", "Pattern2", "000");
			}
			ConfigEntry<int> intEntry;
			{
				intEntry = Config.Bind<int>("Mask", "PatternSizeX", 3);
				if (intEntry.Value < 1) {
					intEntry.Value = 3;
					saveFlag = true;
				}
				ConfigMaskSizeX = intEntry.Value;
			}
			{
				intEntry = Config.Bind<int>("Mask", "PatternSizeY", 3);
				if (intEntry.Value < 1) {
					intEntry.Value = 3;
					saveFlag = true;
				}
				ConfigMaskSizeY = intEntry.Value;
			}
			if (ConfigMask == null || ConfigMask.Length != ConfigMaskSizeY) {
				ConfigMask = new string[ConfigMaskSizeY];
			}
			for (var i = 0; i < ConfigMaskSizeX; ++i) {
				var configDefinition = new ConfigDefinition("Mask", "Pattern" + i);
				var defaultVal = String.Empty.PadLeft(ConfigMaskSizeX, '1');
				var description = i == 0 ? new ConfigDescription("1: Can put the Foundation. 0: Can not put the Foundation.") : ConfigDescription.Empty;
				ConfigEntry<string> stringEntry;
				stringEntry = Config.Bind<String>(configDefinition, defaultVal, description);
				if (stringEntry.Value.Length != ConfigMaskSizeX) {
					Config.Remove(configDefinition);
					stringEntry = Config.Bind<String>(configDefinition, defaultVal, description);
					saveFlag = true;
				}
				ConfigMask[i] = stringEntry.Value;
			}
			if (saveFlag) {
				var OrphanedEntries = (Dictionary<ConfigDefinition, string>)typeof(ConfigFile).GetProperty("OrphanedEntries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Config, null);
				OrphanedEntries.Clear();
				Config.Save();
				FoundationMask.Logger.LogInfo("save config.");
			}
			FoundationMask.Logger.LogInfo("config reloaded.");
		}
	}
}

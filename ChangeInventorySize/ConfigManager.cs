using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Tanukinomori
{
	// https://github.com/BepInEx/BepInEx/blob/master/BepInEx.Core/Configuration/ConfigFile.cs

	public abstract class ConfigManager
	{
		public enum Step { AWAKE, GAME_MAIN_BEGIN }

		private static ConfigManager _instance;

		public static ConfigFile Config { private set; get; }

		protected ConfigManager(ConfigFile config) {
			_instance = this;
			Config = config;
			Config.SaveOnConfigSet = false;
		}

		public static void ConfigReload(Step step) =>
			_instance.ConfigReloadImplements(step);

		protected abstract void ConfigReloadImplements(Step step);

		public static ConfigEntry<T> Bind<T>(ConfigDefinition configDefinition, T defaultValue, ConfigDescription configDescription = null) =>
			Config.Bind<T>(configDefinition, defaultValue, configDescription);

		public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, ConfigDescription configDescription = null) =>
			Config.Bind<T>(section, key, defaultValue, configDescription);

		public static ConfigEntry<T> Bind<T>(string section, string key, T defaultValue, string description) =>
			Config.Bind<T>(section, key, defaultValue, description);

		public static ConfigEntry<T> GetEntry<T>(ConfigDefinition configDefinition) =>
			(ConfigEntry<T>)Config[configDefinition];

		public static ConfigEntry<T> GetEntry<T>(string section, string key) =>
			(ConfigEntry<T>)Config[section, key];

		public static T GetValue<T>(ConfigDefinition configDefinition) =>
			GetEntry<T>(configDefinition).Value;

		public static T GetValue<T>(string section, string key) =>
			GetEntry<T>(section, key).Value;

		public static bool Remove(ConfigDefinition key) =>
			Config.Remove(key);

		public static Dictionary<ConfigDefinition, string> GetOrphanedEntries() =>
			(Dictionary<ConfigDefinition, string>)typeof(ConfigFile).GetProperty("OrphanedEntries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Config, null);

		public static void Save(bool clearOrphanedEntries = true) {
			if (clearOrphanedEntries) {
				GetOrphanedEntries().Clear();
			}
			Config.Save();
			LogManager.Logger.LogInfo("save config.");
		}
	}
}

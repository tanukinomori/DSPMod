using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;

// https://github.com/BepInEx/BepInEx/blob/master/BepInEx.Core/Configuration/ConfigFile.cs

namespace Tanukinomori
{
	public abstract class ConfigManager
	{
		public enum Step { AWAKE, GAME_MAIN_BEGIN, STATE }

		private static ConfigManager instance = null;

		private static Dictionary<ConfigDefinition, string> orphanedEntries = null;

		public static ConfigFile Config { private set; get; } = null;

		protected ConfigManager(ConfigFile config)
		{
			instance = this;
			Config = config;
			Config.SaveOnConfigSet = false;
		}

		public static void CheckConfig(Step step) =>
			instance.CheckConfigImplements(step);

		protected abstract void CheckConfigImplements(Step step);

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

		public static bool UpdateEntry<T>(string section, string key, T value) where T : IComparable
		{
			var entry = GetEntry<T>(section, key);
			if (entry.Value.CompareTo(value) == 0)
			{
				return false;
			}
			entry.Value = value;
			return true;
		}

		public static bool RemoveEntry(ConfigDefinition key) =>
			Config.Remove(key);

		public static Dictionary<ConfigDefinition, string> GetOrphanedEntries()
		{
			if (orphanedEntries == null)
			{
				orphanedEntries = (Dictionary<ConfigDefinition, string>)AccessTools.Property(typeof(ConfigFile), "OrphanedEntries").GetValue(Config, null);
			}
			return orphanedEntries;
		}

		public static void Migration<T>(string newSection, string newKey, T defaultValue, string oldSection, string oldKey)
		{
			GetOrphanedEntries();
			var oldDef = new ConfigDefinition(oldSection, oldKey);
			if (orphanedEntries.TryGetValue(oldDef, out var s))
			{
				Bind(newSection, newKey, defaultValue).SetSerializedValue(s);
				orphanedEntries.Remove(oldDef);
				LogManager.LogInfo($"migration {oldSection}.{oldKey}({s}) => {newSection}.{newKey}");
			}
		}

		public static void Save(bool clearOrphanedEntries = true)
		{
			if (clearOrphanedEntries)
			{
				GetOrphanedEntries().Clear();
			}
			Config.Save();
			LogManager.LogInfo("save config.");
		}
	}
}

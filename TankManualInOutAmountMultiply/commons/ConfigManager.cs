using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;

// https://github.com/BepInEx/BepInEx/blob/master/BepInEx.Core/Configuration/ConfigFile.cs

namespace tanu.TankManualInOutAmountMultiply
{
	public abstract class ConfigManager
	{
		public enum Step
		{
			AWAKE,
			GAME_MAIN_BEGIN
		}

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

		public static ConfigEntry<T> GetEntry<T>(ConfigDefinition configDefinition)
		{
			try
			{
				return (ConfigEntry<T>)Config[configDefinition];
			}
			catch (KeyNotFoundException e)
			{
				LogManager.LogError($"{e.GetType()}: configDefinition={configDefinition}");
				throw;
			}
		}

		public static ConfigEntry<T> GetEntry<T>(string section, string key) =>
			GetEntry<T>(new ConfigDefinition(section, key));

		public static T GetValue<T>(ConfigDefinition configDefinition) =>
			GetEntry<T>(configDefinition).Value;

		public static T GetValue<T>(string section, string key) =>
			GetEntry<T>(section, key).Value;

		public static bool ContainsKey(ConfigDefinition configDefinition) =>
			Config.ContainsKey(configDefinition);

		public static bool ContainsKey(string section, string key) =>
			Config.ContainsKey(new ConfigDefinition(section, key));

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
				orphanedEntries = Traverse.Create(Config).Property<Dictionary<ConfigDefinition, string>>("OrphanedEntries").Value;
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

		public static void Save(bool clearOrphanedEntries = false)
		{
			if (clearOrphanedEntries)
			{
				GetOrphanedEntries().Clear();
			}
			Config.Save();
			LogManager.LogInfo("save config.");
		}

		public static void Clear() =>
			Config.Clear();

		public static void Reload() =>
			Config.Reload();
	}
}

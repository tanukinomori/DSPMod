using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutoReplenishItem
{
	public class AutoReplenishItemConfigManager : ConfigManager
	{
		public AutoReplenishItemConfigManager(ConfigFile Config) : base(Config) {
		}

		private static Regex ITEM_NAME_ID_COUNT_KEY_REGEX = new Regex("ItemNameIdCount[0-9]+", RegexOptions.Compiled);

		private static ConfigDefinition RESET_CONFIG_DEFINITION = new ConfigDefinition("AutoReplenish", "Reset");

		override protected void ConfigReloadImplements(Step step) {
			bool saveFlag = false;
			if (step == Step.AWAKE) {
				Bind<string>("Base", "ModVersion", AutoReplenishItem.ModVersion, "Don't change.");
				var orphanedEntries = ConfigManager.GetOrphanedEntries();
				bool resetFlag = false;
				if (!orphanedEntries.ContainsKey(RESET_CONFIG_DEFINITION)) {
					resetFlag = true;
				}
				Bind<bool>(RESET_CONFIG_DEFINITION, false);
				if (resetFlag) {
					GetEntry<bool>(RESET_CONFIG_DEFINITION).Value = true;
					saveFlag = true;
				}
			} else if (step == Step.GAME_MAIN_BEGIN) {
				ConfigManager.Config.Clear();
				ConfigManager.Config.Reload();
				Bind<bool>(RESET_CONFIG_DEFINITION, false);
				if (ConfigManager.GetValue<bool>(RESET_CONFIG_DEFINITION)) {
					LogManager.Logger.LogInfo("reset AutoReplenish.");
					var orphanedEntries = ConfigManager.GetOrphanedEntries();
					foreach (var definition in new List<ConfigDefinition>(orphanedEntries.Keys)) {
						if (definition.Section == "AutoReplenish" && AutoReplenishItemConfigManager.ITEM_NAME_ID_COUNT_KEY_REGEX.IsMatch(definition.Key)) {
							orphanedEntries.Remove(definition);
						}
					}
					var items = LDB.items.dataArray;
					for (int i = 0; i < items.Length; ++i) {
						var item = items[i];
						orphanedEntries.Add(new ConfigDefinition("AutoReplenish", "ItemNameIdCount" + i), item.Name.Translate() + "/" + item.ID + "/0");
					}
					ConfigManager.GetEntry<bool>(RESET_CONFIG_DEFINITION).Value = false;
					ConfigManager.Save(false);
				} else {
					AutoReplenishItem.AutoReplenishItemMap.Clear();
					var items = LDB.items;
					var orphanedEntries = ConfigManager.GetOrphanedEntries();
					foreach (var definition in new List<ConfigDefinition>(orphanedEntries.Keys)) {
						if (definition.Section == "AutoReplenish" && AutoReplenishItemConfigManager.ITEM_NAME_ID_COUNT_KEY_REGEX.IsMatch(definition.Key)) {
							var itemNameIdCount =orphanedEntries[definition];
							var array = itemNameIdCount.Split('/');
							int count;
							int.TryParse(array[array.Length - 1], out count);
							if (count == 0) {
								continue;
							}
							int id;
							int.TryParse(array[array.Length - 2], out id);
							var item = items.Select(id);
							if (item != null) {
								AutoReplenishItem.AutoReplenishItemMap.Add(id, count);
								LogManager.Logger.LogInfo("add item=" + item.Name.Translate() + ", count=" + count);
							}
						}
					}
				}
			}
			if (saveFlag) {
				Save(false);
			}
			LogManager.Logger.LogInfo("config reloaded.");
		}
	}
}

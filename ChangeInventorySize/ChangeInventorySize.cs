using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using HarmonyLib;

namespace Tanukinomori
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class ChangeInventorySize : BaseUnityPlugin
	{
		public const string ModGuid = "tanu.ChangeInventorySize";
		public const string ModName = "ChangeInventorySize";
		public const string ModVersion = "0.0.1";
		public void Awake()
		{
			LogManager.Logger = base.Logger;
			new ChangeInventorySizeConfigManager(base.Config);
			ConfigManager.ConfigReload(ConfigManager.Step.AWAKE);
			var harmony = new Harmony($"{ModGuid}.Patch");
			harmony.PatchAll(typeof(Patch_GameMain_Begin));
		}
	}
}

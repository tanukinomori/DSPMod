using BepInEx;
using HarmonyLib;
using System.Collections.Generic;

namespace AutoReplenishItem
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class AutoReplenishItem : BaseUnityPlugin
	{
		public const string ModGuid = "jp.co.tanukinomori.dsp.autoreplenishitem";
		public const string ModName = "AutoReplenishItem";
		public const string ModVersion = "0.0.1";

		public static Dictionary<int, int> AutoReplenishItemMap { get; } = new Dictionary<int, int>();

		public void Awake() {
			LogManager.Logger = base.Logger;
			new AutoReplenishItemConfigManager(base.Config);
			ConfigManager.ConfigReload(ConfigManager.Step.AWAKE);
			var harmony = new Harmony("jp.co.tanukinomori.dsp.autoreplenishitem.patch");
			harmony.PatchAll(typeof(Patch_GameMain_Begin));
			harmony.PatchAll(typeof(Patch_UIStorageWindow_OnOpen));
			harmony.PatchAll(typeof(Patch_UIStorageWindow_OnUpdate));
		}
	}
}

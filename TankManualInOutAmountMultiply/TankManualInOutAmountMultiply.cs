using BepInEx;
using HarmonyLib;

namespace tanu.TankManualInOutAmountMultiply
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class TankManualInOutAmountMultiply : BaseUnityPlugin
	{
		public const string ModGuid = "tanu.TankManualInOutAmountMultiply";
		public const string ModName = "TankManualInOutAmountMultiply";
		public const string ModVersion = "0.0.2";

		public static int ConfigMultiValue;
		public static int TankManualInOutMaxAmount;
		public static bool ErrorFlag = false;

		private Harmony harmony;

		public void Awake()
		{
			LogManager.Logger = base.Logger;
			new TankManualInOutAmountMultiplyConfigManager(base.Config);
			ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);
			harmony = new Harmony($"{ModGuid}.Patch");
			harmony.PatchAll(typeof(Patch_GameMain));
			harmony.PatchAll(typeof(Patch_UITankWindow));
		}

		public void OnDestroy()
		{
			harmony.UnpatchAll();
		}
	}
}

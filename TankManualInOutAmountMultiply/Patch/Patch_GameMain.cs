using HarmonyLib;

namespace tanu.TankManualInOutAmountMultiply
{
	[HarmonyPatch(typeof(GameMain))]
	public class Patch_GameMain
	{
		[HarmonyPatch(nameof(GameMain.Begin)), HarmonyPostfix]
		public static void Begin_Postfix()
		{
			ConfigManager.Clear();
			ConfigManager.Reload();
			ConfigManager.CheckConfig(ConfigManager.Step.GAME_MAIN_BEGIN);
		}
	}
}

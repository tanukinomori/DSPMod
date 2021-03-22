using HarmonyLib;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(GameMain))]
	public class Patch_GameMain
	{
		[HarmonyPatch("Begin"), HarmonyPostfix]
		public static void Begin_Postfix()
		{
			ConfigManager.CheckConfig(ConfigManager.Step.GAME_MAIN_BEGIN);
		}

		[HarmonyPatch("Pause"), HarmonyPrefix]
		public static void Pause_Prefix()
		{
			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}
	}
}

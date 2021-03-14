using HarmonyLib;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(GameMain))]
	public class Patch_GameMain
	{
		[HarmonyPatch("Begin"), HarmonyPrefix]
		public static void Begin_Prefix() {
			ConfigManager.ConfigReload(ConfigManager.Step.GAME_MAIN_BEGIN);
		}

		[HarmonyPatch("Pause"), HarmonyPrefix]
		public static void Pause_Prefix() =>
			CruiseAssistUI.Show = false;
	}
}

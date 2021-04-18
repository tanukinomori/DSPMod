using HarmonyLib;

namespace Tanukinomori.Patch
{
	[HarmonyPatch(typeof(UIStarmap))]
	public class Patch_UIStarmap
	{
		[HarmonyPatch("_OnClose"), HarmonyPrefix]
		public static void OnClose_Prefix()
		{
			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}
	}
}

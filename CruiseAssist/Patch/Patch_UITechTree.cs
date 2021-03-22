using HarmonyLib;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(UITechTree))]
	public class Patch_UITechTree
	{
		[HarmonyPatch("_OnOpen"), HarmonyPrefix]
		public static void OnOpen_Prefix()
		{
			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}
	}
}

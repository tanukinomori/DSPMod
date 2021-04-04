using HarmonyLib;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(GameMain))]
	public class Patch_GameMain
	{
		[HarmonyPatch(nameof(GameMain.Pause)), HarmonyPostfix]
		public static void Pause_Postfix()
		{
			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}
	}
}

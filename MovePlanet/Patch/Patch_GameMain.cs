using HarmonyLib;

namespace tanu.MovePlanet
{
	[HarmonyPatch(typeof(GameMain))]
	public class Patch_GameMain
	{
		[HarmonyPatch(nameof(GameMain.Pause)), HarmonyPostfix]
		public static void Pause_Postfix()
		{
			//LogManager.LogInfo(MethodBase.GetCurrentMethod());

			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}
	}
}

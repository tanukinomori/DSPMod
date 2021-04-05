using HarmonyLib;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(UILoadGameWindow))]
	public class Patch_UILoadGameWindow
	{
		[HarmonyPatch("_OnOpen"), HarmonyPostfix]
		public static void OnOpen_Postfix()
		{
			MovePlanet.LoadGameWindowActive = true;
		}

		[HarmonyPatch("_OnClose"), HarmonyPostfix]
		public static void OnClose_Postfix()
		{
			MovePlanet.LoadGameWindowActive = false;
		}
	}
}

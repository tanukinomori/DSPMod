using HarmonyLib;
using System;
using System.Reflection;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(UILoadGameWindow))]
	public class Patch_UILoadGameWindow
	{
		[HarmonyPatch("_OnOpen"), HarmonyPostfix]
		public static void OnOpen_Postfix()
		{
			MovePlanet.LoadGameWindowActive = true;

			if (MovePlanet.ErrorFlag)
			{
				throw new SystemException("MovePlanet - An error has occurred.");
			}
		}

		[HarmonyPatch("_OnClose"), HarmonyPostfix]
		public static void OnClose_Postfix()
		{
			//LogManager.LogInfo(MethodBase.GetCurrentMethod());

			MovePlanet.LoadGameWindowActive = false;
		}

		[HarmonyPatch("DoLoadSelectedGame"), HarmonyPrefix]
		public static void DoLoadSelectedGame_Prefix()
		{
			//LogManager.LogInfo(MethodBase.GetCurrentMethod());

			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}
	}
}

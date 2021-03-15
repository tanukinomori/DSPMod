﻿using HarmonyLib;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(UITechTree))]
	public class Patch_UITechTree
	{
		[HarmonyPatch("_OnOpen"), HarmonyPrefix]
		public static void OnOpen_Prefix()
		{
			CruiseAssist.TechTreeShow = true;
			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}

		[HarmonyPatch("_OnClose"), HarmonyPrefix]
		public static void OnClose_Prefix()
		{
			CruiseAssist.TechTreeShow = false;
		}
	}
}
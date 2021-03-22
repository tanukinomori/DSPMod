using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

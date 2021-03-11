using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutoReplenishItem
{
	[HarmonyPatch(typeof(GameMain), "Begin")]
	class Patch_GameMain_Begin
	{
		[HarmonyPrefix]
		static void GameMain_Begin_Prefix() {
			ConfigManager.ConfigReload(ConfigManager.Step.GAME_MAIN_BEGIN);
		}
	}
}

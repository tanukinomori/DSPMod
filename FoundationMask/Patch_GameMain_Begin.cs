using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;

namespace FoundationMask
{
	[HarmonyPatch(typeof(GameMain), "Begin")]
	class Patch_GameMain_Begin
	{
		[HarmonyPrefix]
		public static void GameMain_Begin_Prefix() {
			FoundationMask.Config.Clear();
			FoundationMask.Config.Reload();
		}
	}
}

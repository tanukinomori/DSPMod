using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;

namespace TankManualInOutAmountMultiply
{
	[HarmonyPatch(typeof(GameMain), "Begin")]
	class Patch_GameMain_Begin
	{
		[HarmonyPrefix]
		static void GameMain_Begin_Prefix() {
			TankManualInOutAmountMultiply.Config.Clear();
			TankManualInOutAmountMultiply.Config.Reload();
		}
	}
}

using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(GameMain), "Begin")]
	public class Patch_GameMain_Begin
	{
		[HarmonyPrefix]
		public static void GameMain_Begin_Prefix()
		{
			ConfigManager.ConfigReload(ConfigManager.Step.GAME_MAIN_BEGIN);
			GameMain.mainPlayer.package.SetSize(ConfigManager.GetValue<int>("Setting", "InventorySize"));
		}
	}
}

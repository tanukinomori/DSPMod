using HarmonyLib;
using UnityEngine;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(PlayerMove_Sail))]
	public class Patch_PlayerMoveSail
	{
		[HarmonyPatch("GameTick"), HarmonyPrefix]
		public static void GameTick_Prefix(PlayerMove_Sail __instance)
		{
			CruiseAssist.State = CruiseAssistState.INACTIVE;
			var player = __instance.player;
			if (!player.sailing)
			{
				return;
			}
			if (CruiseAssist.ReticuleTargetPlanet != null)
			{
				CruiseAssist.State = CruiseAssistState.TO_PLANET_RETICULE;
				var playerToPlanet = CruiseAssist.ReticuleTargetPlanet.uPosition - player.uPosition;
				var angle = Vector3.Angle(playerToPlanet, player.uVelocity);
				var t = 1.6f / Mathf.Max(10f, angle);
				var magnitude = player.controller.actionSail.visual_uvel.magnitude;
				player.uVelocity = Vector3.Slerp(player.uVelocity, playerToPlanet.normalized * magnitude, t);
			}
			else if (player.warping && CruiseAssist.ReticuleTargetStar != null)
			{
				CruiseAssist.State = CruiseAssistState.TO_STAR_RETICULE;
				var playerToStar = CruiseAssist.ReticuleTargetStar.uPosition - player.uPosition;
				var angle = Vector3.Angle(playerToStar, player.uVelocity);
				var t = 1.6f / Mathf.Max(10f, angle);
				var magnitude = player.controller.actionSail.visual_uvel.magnitude;
				player.uVelocity = Vector3.Slerp(player.uVelocity, playerToStar.normalized * magnitude, t);
			}
		}
	}
}

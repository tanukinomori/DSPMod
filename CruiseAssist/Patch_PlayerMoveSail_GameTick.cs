using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(PlayerMove_Sail), "GameTick")]
	public class Patch_PlayerMoveSail_GameTick
	{
		[HarmonyPrefix]
		public static void PlayerMoveSail_GameTick_Prefix(PlayerMove_Sail __instance) {
			var player = __instance.player;
			if (player.movementState < EMovementState.Sail) {
				return;
			}
			if (CruiseAssist.targetPlanet != null) {
				var playerToPlanet = CruiseAssist.targetPlanet.uPosition - player.uPosition;
				//var a = VectorLF3.AngleRAD(player.uVelocity, playerToPlanet);
				//var d = playerToPlanet.magnitude - (double)CruiseAssist.targetPlanet.realRadius;
				//if (d < 10000.0) {
				//	LogManager.Logger.LogInfo(((int)(d + 0.5)).ToString() + "m - " + a.ToString("0.000"));
				//} else if (d < 600000.0) {
				//	LogManager.Logger.LogInfo((d / 40000.0).ToString("0.00") + "AU - " + a.ToString("0.000"));
				//} else {
				//	LogManager.Logger.LogInfo((d / 2400000.0).ToString("0.00") + "光年 - " + a.ToString("0.000"));
				//}
				var angle = Vector3.Angle(playerToPlanet, player.uVelocity);
				var t = 1.6f / Mathf.Max(10f, angle);
				var magnitude = player.controller.actionSail.visual_uvel.magnitude;
				player.uVelocity = Vector3.Slerp(player.uVelocity, playerToPlanet.normalized * magnitude, t);
			} else if (player.warping && CruiseAssist.targetStar != null) {
				var playerToStar = CruiseAssist.targetStar.uPosition - player.uPosition;
				//var a = VectorLF3.AngleRAD(player.uVelocity, playerToStar);
				//var d = playerToStar.magnitude;
				//if (d < 10000.0) {
				//	LogManager.Logger.LogInfo(((int)(d + 0.5)).ToString() + "m - " + a.ToString("0.000"));
				//} else if (d < 600000.0) {
				//	LogManager.Logger.LogInfo((d / 40000.0).ToString("0.00") + "AU - " + a.ToString("0.000"));
				//} else {
				//	LogManager.Logger.LogInfo((d / 2400000.0).ToString("0.00") + "光年 - " + a.ToString("0.000"));
				//}
				var angle = Vector3.Angle(playerToStar, player.uVelocity);
				var t = 1.6f / Mathf.Max(10f, angle);
				var magnitude = player.controller.actionSail.visual_uvel.magnitude;
				player.uVelocity = Vector3.Slerp(player.uVelocity, playerToStar.normalized * magnitude, t);
			}
		}
	}
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(PlayerMove_Sail), "GameTick")]
	class Patch_PlayerMoveSail_GameTick
	{
		[HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

			CodeMatcher matcher = new CodeMatcher(instructions);

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0)); // 0

			matcher.
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0)).
				InsertAndAdvance(Transpilers.EmitDelegate<Action<PlayerMove_Sail>>(
					playerMoveSail => {
						var player = playerMoveSail.player;
						if (player.movementState < EMovementState.Sail) {
							return;
						}
						if (player.warping) {
							if (CruiseAssist.targetStar == null) {
								return;
							}
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
						} else {
							if (CruiseAssist.targetPlanet == null) {
								return;
							}
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
						}
					}));

			return matcher.InstructionEnumeration();
		}
	}
}

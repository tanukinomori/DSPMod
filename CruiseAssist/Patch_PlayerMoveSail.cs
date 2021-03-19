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

			var astroId = GameMain.mainPlayer.navigation.indicatorAstroId;

			if (CruiseAssist.SelectTargetAstroId != astroId)
			{
				CruiseAssist.SelectTargetAstroId = astroId;
				if (astroId % 100 != 0)
				{
					CruiseAssist.SelectTargetPlanet = GameMain.galaxy.PlanetById(astroId);
					CruiseAssist.SelectTargetStar = CruiseAssist.SelectTargetPlanet.star;
				}
				else
				{
					CruiseAssist.SelectTargetPlanet = null;
					CruiseAssist.SelectTargetStar = GameMain.galaxy.StarById(astroId / 100);
				}
			}

			CruiseAssist.TargetStar = null;
			CruiseAssist.TargetPlanet = null;

			if (CruiseAssist.SelectTargetStar != null)
			{
				// 星系を選択

				if (GameMain.localStar != null && CruiseAssist.SelectTargetStar.id == GameMain.localStar.id)
				{
					// 選択した星系の中に居るとき

					if (CruiseAssist.SelectTargetPlanet == null && GameMain.localPlanet != null)
					{
						// 惑星を未選択で何れかの惑星に居るとき、選択を解除する
						CruiseAssist.SelectTargetStar = null;
						CruiseAssist.SelectTargetAstroId = 0;
						GameMain.mainPlayer.navigation.indicatorAstroId = 0;
						return;
					}

					if (CruiseAssist.SelectTargetPlanet != null)
					{
						// 惑星を選択

						if (GameMain.localPlanet != null && CruiseAssist.SelectTargetPlanet.id == GameMain.localPlanet.id)
						{
							// 選択した惑星に居るとき、選択を解除する
							CruiseAssist.SelectTargetStar = null;
							CruiseAssist.SelectTargetPlanet = null;
							CruiseAssist.SelectTargetAstroId = 0;
							GameMain.mainPlayer.navigation.indicatorAstroId = 0;
							return;
						}

						// 対象とする
						CruiseAssist.TargetPlanet = CruiseAssist.SelectTargetPlanet;
					}
					else if (CruiseAssist.ReticuleTargetPlanet != null)
					{
						// レティクルが惑星を向いているとき、対象とする
						CruiseAssist.TargetPlanet = CruiseAssist.ReticuleTargetPlanet;
					}
				}
				else
				{
					// 選択した星系の外に居るとき

					// 選択した星系を対象とする
					CruiseAssist.TargetStar = CruiseAssist.SelectTargetStar;
				}
			}
			else
			{
				// 星系も惑星も未選択

				if (CruiseAssist.ReticuleTargetPlanet != null)
				{
					// レティクルが惑星を向いているとき、対象とする
					CruiseAssist.TargetPlanet = CruiseAssist.ReticuleTargetPlanet;
				}
				else if (CruiseAssist.ReticuleTargetStar != null)
				{
					// レティクルが星系を向いているとき、対象とする
					CruiseAssist.TargetStar = CruiseAssist.ReticuleTargetStar;
				}
			}

			VectorLF3 targetPos;

			if (CruiseAssist.TargetPlanet != null)
			{
				CruiseAssist.State = CruiseAssistState.TO_PLANET;
				targetPos = CruiseAssist.TargetPlanet.uPosition - player.uPosition;
			}
			else if (CruiseAssist.TargetStar != null)
			{
				CruiseAssist.State = CruiseAssistState.TO_STAR;
				targetPos = CruiseAssist.TargetStar.uPosition - player.uPosition;
			}
			else
			{
				return;
			}

			var angle = Vector3.Angle(targetPos, player.uVelocity);
			var t = 1.6f / Mathf.Max(10f, angle);
			var speed = player.controller.actionSail.visual_uvel.magnitude;
			player.uVelocity = Vector3.Slerp(player.uVelocity, targetPos.normalized * speed, t);
		}
	}
}

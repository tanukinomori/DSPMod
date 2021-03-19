using UnityEngine;
using System.Linq;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace Tanukinomori
{
	public class CruiseAssistStarListUI
	{
		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, 540f, 600f);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		private static Vector2 scrollPos = Vector2.zero;

		public static void OnGUI()
		{
			Rect = GUILayout.Window(99030292, Rect, WindowFunction, "CruiseAssist - StarList");

			if (Rect.x < 0)
			{
				Rect.x = 0;
			}
			else if (Screen.width < Rect.xMax)
			{
				Rect.x = Screen.width - Rect.width;
			}

			if (Rect.y < 0)
			{
				Rect.y = 0;
			}
			else if (Screen.height < Rect.yMax)
			{
				Rect.y = Screen.height - Rect.height;
			}

			if (lastCheckWindowLeft != float.MinValue)
			{
				if (Rect.x != lastCheckWindowLeft || Rect.y != lastCheckWindowTop)
				{
					nextCheckGameTick = GameMain.gameTick + 300;
				}
			}

			lastCheckWindowLeft = Rect.x;
			lastCheckWindowTop = Rect.y;

			if (nextCheckGameTick <= GameMain.gameTick)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
				nextCheckGameTick = long.MaxValue;
			}
		}

		public static void WindowFunction(int windowId)
		{
			GUI.skin.label.fontSize = 16;

			GUILayout.BeginVertical();

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			GameMain.galaxy.stars.Select(star => new Tuple<StarData, double>(star, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).OrderBy(tuple => tuple.v2).Do(tuple =>
			{
				var star = tuple.v1;
				var starName = CruiseAssist.GetStarName(star);
				if (GameMain.localStar != null && star.id == GameMain.localStar.id)
				{
					GameMain.localStar.planets.
						Select(planet => new Tuple<object, double>(planet, (planet.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
						AddItem(new Tuple<object, double>(star, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
						OrderBy(tuple2 => tuple2.v2).
						Do(tuple2 =>
						{
							var planet = tuple2.v1 as PlanetData;
							GUILayout.BeginHorizontal();
							GUI.color = Color.white;
							if (tuple2.v1 is StarData)
							{
								if (CruiseAssist.SelectTargetPlanet == null && CruiseAssist.SelectTargetStar != null && star.id == CruiseAssist.SelectTargetStar.id)
								{
									GUI.color = Color.cyan;
								}
								GUILayout.Label(starName);
							}
							else
							{
								if (CruiseAssist.SelectTargetPlanet != null && planet.id == CruiseAssist.SelectTargetPlanet.id)
								{
									GUI.color = Color.cyan;
								}
								GUILayout.Label(starName + " - " + CruiseAssist.GetPlanetName(planet));

							}
							GUILayout.FlexibleSpace();
							GUILayout.Label(CruiseAssistMainUI.RangeToString(tuple2.v2));
							GUI.color = Color.white;
							if (GUILayout.Button("SET"))
							{
								if (tuple2.v1 is StarData)
								{
									CruiseAssist.SelectTargetStar = star;
									CruiseAssist.SelectTargetPlanet = null;
								}
								else
								{
									CruiseAssist.SelectTargetStar = star;
									CruiseAssist.SelectTargetPlanet = planet;
								}
							}
							GUILayout.EndHorizontal();
						});
				}
				else
				{
					GUILayout.BeginHorizontal();
					GUI.color = Color.white;
					if (CruiseAssist.SelectTargetStar != null && star.id == CruiseAssist.SelectTargetStar.id)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label(starName);
					GUILayout.FlexibleSpace();
					GUILayout.Label(CruiseAssistMainUI.RangeToString(tuple.v2));
					GUI.color = Color.white;
					if (GUILayout.Button("SET"))
					{
						CruiseAssist.SelectTargetStar = star;
						CruiseAssist.SelectTargetPlanet = null;
					}
					GUI.color = Color.white;
					GUILayout.EndHorizontal();
				}
			});

			GUILayout.EndScrollView();

			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
			{
				CruiseAssist.SelectTargetStar = null;
				CruiseAssist.SelectTargetPlanet = null;
			}

			if (GUILayout.Button("Close", GUILayout.ExpandWidth(false)))
			{
				Show = false;
			}

			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

using HarmonyLib;
using System;
using System.Linq;
using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistStarListUI
	{
		private static int wIdx = 0;

		public static float WindowWidth = 400f;
		public static float WindowHeight = 480f;

		public static bool[] Show = { false, false };
		public static Rect[] Rect = {
			new Rect(0f, 0f, WindowWidth, WindowHeight),
			new Rect(0f, 0f, WindowWidth, WindowHeight) };
		public static int ListSelected;

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		private static Vector2 scrollPos = Vector2.zero;

		public static void OnGUI()
		{
			wIdx = CruiseAssistMainUI.wIdx;

			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect[wIdx] = GUILayout.Window(99030292, Rect[wIdx], WindowFunction, "CruiseAssist - StarList", windowStyle);

			var scale = CruiseAssistMainUI.Scale / 100.0f;

			if (Screen.width / scale < Rect[wIdx].xMax)
			{
				Rect[wIdx].x = Screen.width / scale - Rect[wIdx].width;
			}
			if (Rect[wIdx].x < 0)
			{
				Rect[wIdx].x = 0;
			}

			if (Screen.height / scale < Rect[wIdx].yMax)
			{
				Rect[wIdx].y = Screen.height / scale - Rect[wIdx].height;
			}
			if (Rect[wIdx].y < 0)
			{
				Rect[wIdx].y = 0;
			}

			if (lastCheckWindowLeft != float.MinValue)
			{
				if (Rect[wIdx].x != lastCheckWindowLeft || Rect[wIdx].y != lastCheckWindowTop)
				{
					nextCheckGameTick = GameMain.gameTick + 300;
				}
			}

			lastCheckWindowLeft = Rect[wIdx].x;
			lastCheckWindowTop = Rect[wIdx].y;

			if (nextCheckGameTick <= GameMain.gameTick)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
				nextCheckGameTick = long.MaxValue;
			}
		}

		public static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			var mainWindowStyleButtonStyle = new GUIStyle(GUI.skin.button);
			mainWindowStyleButtonStyle.fixedWidth = 80;
			mainWindowStyleButtonStyle.fixedHeight = 20;
			mainWindowStyleButtonStyle.fontSize = 12;

			string[] texts = { "Normal", "History" };
			var selected = GUILayout.Toolbar(ListSelected, texts, mainWindowStyleButtonStyle);
			if (selected != ListSelected)
			{
				ListSelected = selected;
				nextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.EndHorizontal();

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			var nameLabelStyle = new GUIStyle(GUI.skin.label);
			nameLabelStyle.fixedWidth = 240;
			nameLabelStyle.stretchHeight = true;
			nameLabelStyle.fontSize = 14;
			nameLabelStyle.alignment = TextAnchor.MiddleLeft;

			var nRangeLabelStyle = new GUIStyle(GUI.skin.label);
			nRangeLabelStyle.fixedWidth = 60;
			nRangeLabelStyle.fixedHeight = 20;
			nRangeLabelStyle.fontSize = 14;
			nRangeLabelStyle.alignment = TextAnchor.MiddleRight;
			var hRangeLabelStyle = new GUIStyle(nRangeLabelStyle);
			hRangeLabelStyle.fixedHeight = 40;

			var nSetTargetButtonStyle = new GUIStyle(GUI.skin.button);
			nSetTargetButtonStyle.fixedWidth = 40;
			nSetTargetButtonStyle.fixedHeight = 18;
			nSetTargetButtonStyle.margin.top = 6;
			nSetTargetButtonStyle.fontSize = 12;
			var hSetTargetButtonStyle = new GUIStyle(nSetTargetButtonStyle);
			hSetTargetButtonStyle.margin.top = 6;

			if (ListSelected == 0)
			{
				GameMain.galaxy.stars.Select(star => new Tuple<StarData, double>(star, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).OrderBy(tuple => tuple.v2).Do(tuple =>
				{
					var star = tuple.v1;
					var range = tuple.v2;
					var starName = CruiseAssist.GetStarName(star);
					StarData star2 = null;
					if (GameMain.localStar != null && star.id == GameMain.localStar.id)
					{
						star2 = GameMain.localStar;
					}
					else if (CruiseAssist.SelectTargetStar != null && star.id == CruiseAssist.SelectTargetStar.id && GameMain.history.universeObserveLevel >= (range >= 14400000.0 ? 4 : 3))
					{
						star2 = star;
					}
					if (star2 != null)
					{
						star2.planets.
							Select(planet => new Tuple<PlanetData, double>(planet, (planet.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
							AddItem(new Tuple<PlanetData, double>(null, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
							OrderBy(tuple2 => tuple2.v2).
							Do(tuple2 =>
							{
								GUILayout.BeginHorizontal();

								var planet = tuple2.v1;
								nameLabelStyle.normal.textColor = Color.white;
								nRangeLabelStyle.normal.textColor = Color.white;
								float textHeight;

								if (planet == null)
								{
									if (CruiseAssist.SelectTargetPlanet == null && CruiseAssist.SelectTargetStar != null && star.id == CruiseAssist.SelectTargetStar.id)
									{
										nameLabelStyle.normal.textColor = Color.cyan;
										nRangeLabelStyle.normal.textColor = Color.cyan;
									}
									GUILayout.Label(starName, nameLabelStyle);
									textHeight = GUILayoutUtility.GetLastRect().height;
								}
								else
								{
									if (CruiseAssist.SelectTargetPlanet != null && planet.id == CruiseAssist.SelectTargetPlanet.id)
									{
										nameLabelStyle.normal.textColor = Color.cyan;
										nRangeLabelStyle.normal.textColor = Color.cyan;
									}
									GUILayout.Label(starName + " - " + CruiseAssist.GetPlanetName(planet), nameLabelStyle);
									textHeight = GUILayoutUtility.GetLastRect().height;
								}

								GUILayout.FlexibleSpace();

								GUILayout.Label(CruiseAssistMainUI.RangeToString(range), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

								if (GUILayout.Button("SET", textHeight < 30 ? nSetTargetButtonStyle : hSetTargetButtonStyle))
								{
									SelectStar(star, planet);
								}

								GUILayout.EndHorizontal();
							});
					}
					else
					{
						GUILayout.BeginHorizontal();

						nameLabelStyle.normal.textColor = Color.white;
						nRangeLabelStyle.normal.textColor = Color.white;

						if (CruiseAssist.SelectTargetStar != null && star.id == CruiseAssist.SelectTargetStar.id)
						{
							nameLabelStyle.normal.textColor = Color.cyan;
							nRangeLabelStyle.normal.textColor = Color.cyan;
						}

						GUILayout.Label(starName, nameLabelStyle);

						GUILayout.FlexibleSpace();

						GUILayout.Label(CruiseAssistMainUI.RangeToString(range), nRangeLabelStyle);

						if (GUILayout.Button("SET", nSetTargetButtonStyle))
						{
							SelectStar(star, null);
						}

						GUILayout.EndHorizontal();
					}
				});
			}
			else if (ListSelected == 1)
			{
				bool highlighted = false;

				CruiseAssist.History.Reverse<int>().Do(id =>
				{
					GUILayout.BeginHorizontal();

					var planet = GameMain.galaxy.PlanetById(id);
					var star = planet.star;
					var starName = CruiseAssist.GetStarName(star);
					var range = (planet.uPosition - GameMain.mainPlayer.uPosition).magnitude;
					nameLabelStyle.normal.textColor = Color.white;
					nRangeLabelStyle.normal.textColor = Color.white;
					float textHeight;

					if (!highlighted && CruiseAssist.SelectTargetPlanet != null && planet.id == CruiseAssist.SelectTargetPlanet.id)
					{
						nameLabelStyle.normal.textColor = Color.cyan;
						nRangeLabelStyle.normal.textColor = Color.cyan;
						highlighted = true;
					}

					GUILayout.Label(starName + " - " + CruiseAssist.GetPlanetName(planet), nameLabelStyle);
					textHeight = GUILayoutUtility.GetLastRect().height;

					GUILayout.FlexibleSpace();

					GUILayout.Label(CruiseAssistMainUI.RangeToString(range), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

					if (GUILayout.Button("SET", textHeight < 30 ? nSetTargetButtonStyle : hSetTargetButtonStyle))
					{
						SelectStar(star, planet);
					}

					GUILayout.EndHorizontal();
				});
			}

			GUILayout.EndScrollView();

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			var closeButtonStyle = new GUIStyle(GUI.skin.button);
			closeButtonStyle.fixedWidth = 80;
			closeButtonStyle.fixedHeight = 20;
			closeButtonStyle.fontSize = 12;

			if (GUILayout.Button("Close", closeButtonStyle))
			{
				Show[wIdx] = false;
			}

			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		public static void SelectStar(StarData star, PlanetData planet)
		{
			CruiseAssist.SelectTargetStar = star;
			CruiseAssist.SelectTargetPlanet = planet;

			if (planet != null)
			{
				GameMain.mainPlayer.navigation.indicatorAstroId = planet.id;
			}
			else if (star != null)
			{
				GameMain.mainPlayer.navigation.indicatorAstroId = star.id * 100;
			}
			else
			{
				GameMain.mainPlayer.navigation.indicatorAstroId = 0;
			}

			CruiseAssist.SelectTargetAstroId = GameMain.mainPlayer.navigation.indicatorAstroId;
		}
	}
}

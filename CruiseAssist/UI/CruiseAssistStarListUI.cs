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
		public static int ListSelected = 0;
		public static int[] actionSelected = { 0, 0, 0 };

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

			string[] texts = { "Normal", "History", "Bookmark" };
			var selected = GUILayout.Toolbar(ListSelected, texts, mainWindowStyleButtonStyle);
			if (selected != ListSelected)
			{
				ListSelected = selected;
				nextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.EndHorizontal();

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			string[,] listButtonActionName =
			{
				// Normal
				{ "SET","ADD" },
				// History
				{ "SET","ADD" },
				// Bookmark
				{ "SET","DEL" },
			};

			var actionName = listButtonActionName[ListSelected, actionSelected[ListSelected]];

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
			hSetTargetButtonStyle.margin.top = 16;

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
								var range2 = tuple2.v2;
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
									var text = starName;
									GUILayout.Label(text, nameLabelStyle);
									textHeight = nameLabelStyle.CalcHeight(new GUIContent(text), nameLabelStyle.fixedWidth);
								}
								else
								{
									if (CruiseAssist.SelectTargetPlanet != null && planet.id == CruiseAssist.SelectTargetPlanet.id)
									{
										nameLabelStyle.normal.textColor = Color.cyan;
										nRangeLabelStyle.normal.textColor = Color.cyan;
									}
									var text = starName + " - " + CruiseAssist.GetPlanetName(planet);
									GUILayout.Label(text, nameLabelStyle);
									textHeight = nameLabelStyle.CalcHeight(new GUIContent(text), nameLabelStyle.fixedWidth);
								}

								GUILayout.FlexibleSpace();

								GUILayout.Label(CruiseAssistMainUI.RangeToString(planet == null ? range : range2), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

								if (GUILayout.Button(actionSelected[ListSelected] == 1 && planet == null ? "-" : actionName, textHeight < 30 ? nSetTargetButtonStyle : hSetTargetButtonStyle))
								{
									if (actionSelected[ListSelected] == 0)
									{
										SelectStar(star, planet);
									}
									else if (planet != null && !CruiseAssist.Bookmark.Contains(planet.id))
									{
										if (CruiseAssist.Bookmark.Count <= 128)
										{
											CruiseAssist.Bookmark.Add(planet.id);
											nextCheckGameTick = GameMain.gameTick + 300;
										}
									}
								}

								GUILayout.EndHorizontal();
							});
					}
					else
					{
						GUILayout.BeginHorizontal();
						float textHeight;

						nameLabelStyle.normal.textColor = Color.white;
						nRangeLabelStyle.normal.textColor = Color.white;

						if (CruiseAssist.SelectTargetStar != null && star.id == CruiseAssist.SelectTargetStar.id)
						{
							nameLabelStyle.normal.textColor = Color.cyan;
							nRangeLabelStyle.normal.textColor = Color.cyan;
						}

						var text = starName;
						GUILayout.Label(starName, nameLabelStyle);
						textHeight = nameLabelStyle.CalcHeight(new GUIContent(text), nameLabelStyle.fixedWidth);

						GUILayout.FlexibleSpace();

						GUILayout.Label(CruiseAssistMainUI.RangeToString(range), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

						if (GUILayout.Button(actionSelected[ListSelected] == 1 ? "-" : actionName, textHeight < 30 ? nSetTargetButtonStyle : hSetTargetButtonStyle))
						{
							if (actionSelected[ListSelected] == 0)
							{
								SelectStar(star, null);
							}
						}

						GUILayout.EndHorizontal();
					}
				});
			}
			else if (ListSelected == 1 || ListSelected == 2)
			{
				bool highlighted = false;

				var list = ListSelected == 1 ? CruiseAssist.History : CruiseAssist.Bookmark;

				list.Reverse<int>().Do(id =>
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

					var text = starName + " - " + CruiseAssist.GetPlanetName(planet);
					GUILayout.Label(text, nameLabelStyle);
					textHeight = nameLabelStyle.CalcHeight(new GUIContent(text), nameLabelStyle.fixedWidth);

					GUILayout.FlexibleSpace();

					GUILayout.Label(CruiseAssistMainUI.RangeToString(range), textHeight < 30 ? nRangeLabelStyle : hRangeLabelStyle);

					if (GUILayout.Button(actionName, textHeight < 30 ? nSetTargetButtonStyle : hSetTargetButtonStyle))
					{
						if (actionSelected[ListSelected] == 0)
						{
							SelectStar(star, planet);
						}
						else
						{
							if (ListSelected == 1)
							{
								if (!CruiseAssist.Bookmark.Contains(planet.id))
								{
									if (CruiseAssist.Bookmark.Count <= 128)
									{
										CruiseAssist.Bookmark.Add(planet.id);
										nextCheckGameTick = GameMain.gameTick + 300;
									}
								}
							}
							else
							{
								CruiseAssist.Bookmark.Remove(planet.id);
								nextCheckGameTick = GameMain.gameTick + 300;
							}
						}
					}

					GUILayout.EndHorizontal();
				});
			}

			GUILayout.EndScrollView();

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();

			var buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fixedWidth = 80;
			buttonStyle.fixedHeight = 20;
			buttonStyle.fontSize = 12;

			string[,] listButtonModeName =
			{
				// Normal
				{ "Target","Bookmark" },
				// History
				{ "Target","Bookmark" },
				// Bookmark
				{ "Target","Delete" },
			};

			if (GUILayout.Button(listButtonModeName[ListSelected, actionSelected[ListSelected]], buttonStyle))
			{
				++actionSelected[ListSelected];
				actionSelected[ListSelected] %= 2;
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Close", buttonStyle))
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

using HarmonyLib;
using System;
using System.Collections.Generic;
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

			string[][] listButtonActionName =
			{
				// Normal
				new string[] { "SET", "ADD" },
				// History
				new string[] { "SET", "ADD" },
				// Bookmark
				new string[] { "SET", null, "DEL" },
			};

			var actionName = listButtonActionName[ListSelected][actionSelected[ListSelected]];

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

			var nActionButtonStyle = new GUIStyle(GUI.skin.button);
			nActionButtonStyle.fixedWidth = 40;
			nActionButtonStyle.fixedHeight = 18;
			nActionButtonStyle.margin.top = 6;
			nActionButtonStyle.fontSize = 12;
			var hActionButtonStyle = new GUIStyle(nActionButtonStyle);
			hActionButtonStyle.margin.top = 16;

			var nSortButtonStyle = new GUIStyle(GUI.skin.button);
			nSortButtonStyle.fixedWidth = 20;
			nSortButtonStyle.fixedHeight = 18;
			nSortButtonStyle.margin.top = 6;
			nSortButtonStyle.fontSize = 12;
			var hSortButtonStyle = new GUIStyle(nSortButtonStyle);
			hSortButtonStyle.margin.top = 16;

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

								if (GUILayout.Button(actionSelected[ListSelected] == 1 && planet == null ? "-" : actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
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

						if (GUILayout.Button(actionSelected[ListSelected] == 1 ? "-" : actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
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

				var list = ListSelected == 1 ? CruiseAssist.History.Reverse<int>() : CruiseAssist.Bookmark.ToList();

				list.Do(id =>
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

					if (ListSelected == 2 && actionSelected[ListSelected] == 1)
					{
						// BookmarkのSort

						var index = CruiseAssist.Bookmark.IndexOf(id);
						bool first = index == 0;
						bool last = index == CruiseAssist.Bookmark.Count - 1;

						if (GUILayout.Button(last ? "-" :  "↓", textHeight < 30 ? nSortButtonStyle : hSortButtonStyle) && !last)
						{
							CruiseAssist.Bookmark.RemoveAt(index);
							CruiseAssist.Bookmark.Insert(index + 1, id);
						}
						if (GUILayout.Button(first ? "-" : "↑", textHeight < 30 ? nSortButtonStyle : hSortButtonStyle) && !first)
						{
							CruiseAssist.Bookmark.RemoveAt(index);
							CruiseAssist.Bookmark.Insert(index - 1, id);
						}
					}
					else
					{
						if (GUILayout.Button(actionName, textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
						{
							if (actionSelected[ListSelected] == 0)
							{
								// 0番目(SET)を押したとき、対応する惑星を選択
								SelectStar(star, planet);
							}
							else if (actionSelected[ListSelected] == 1)
							{
								// 1番目を押したとき

								if (ListSelected == 1)
								{
									// History(1番目はADD)のとき

									if (!CruiseAssist.Bookmark.Contains(planet.id))
									{
										if (CruiseAssist.Bookmark.Count <= 128)
										{
											CruiseAssist.Bookmark.Add(planet.id);
											nextCheckGameTick = GameMain.gameTick + 300;
										}
									}
								}
							}
							else if (actionSelected[ListSelected] == 2)
							{
								// 2番目を押したとき

								if (ListSelected == 2)
								{
									// Bookmark(2番目はDEL)のとき

									CruiseAssist.Bookmark.Remove(planet.id);
									nextCheckGameTick = GameMain.gameTick + 300;
								}
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

			string[][] listButtonModeName =
			{
				// Normal
				new string[] { "Target", "Bookmark" },
				// History
				new string[] { "Target", "Bookmark" },
				// Bookmark
				new string[] { "Target", "Sort", "Delete" },
			};

			if (GUILayout.Button(listButtonModeName[ListSelected][actionSelected[ListSelected]], buttonStyle))
			{
				++actionSelected[ListSelected];
				actionSelected[ListSelected] %= listButtonModeName[ListSelected].Length;
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

			var uiGame = UIRoot.instance.uiGame;

			if (CruiseAssist.SelectFocusFlag && uiGame.starmap.active)
			{
				if (star != null)
				{
					var uiStar = uiGame.starmap.starUIs.Where(s => s.star.id == star.id).First();
					UIStarmap_OnStarClick(uiGame.starmap, uiStar);
					uiGame.starmap.OnCursorFunction2Click(0);
				}
				if (planet != null)
				{
					var uiPlanet = uiGame.starmap.planetUIs.Where(p => p.planet.id == planet.id).First();
					UIStarmap_OnPlanetClick(uiGame.starmap, uiPlanet);
					uiGame.starmap.OnCursorFunction2Click(0);
				}
			}

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

		private static void UIStarmap_OnStarClick(UIStarmap starmap, UIStarmapStar star)
		{
			var starmapTraverse = Traverse.Create(starmap);
			if (starmap.focusStar != star)
			{
				if (starmap.viewPlanet != null || (starmap.viewStar != null && star.star != starmap.viewStar))
				{
					starmap.screenCameraController.DisablePositionLock();
				}
				starmap.focusPlanet = null;
				starmap.focusStar = star;
				starmapTraverse.Field("_lastClickTime").SetValue(0.0);
			}
			starmapTraverse.Field("forceUpdateCursorView").SetValue(true);
		}

		private static void UIStarmap_OnPlanetClick(UIStarmap starmap, UIStarmapPlanet planet)
		{
			var starmapTraverse = Traverse.Create(starmap);
			if (starmap.focusPlanet != planet)
			{
				if ((starmap.viewPlanet != null && planet.planet != starmap.viewPlanet) || starmap.viewStar != null)
				{
					starmap.screenCameraController.DisablePositionLock();
				}
				starmap.focusPlanet = planet;
				starmap.focusStar = null;
				starmapTraverse.Field("_lastClickTime").SetValue(0.0);
			}
			starmapTraverse.Field("forceUpdateCursorView").SetValue(true);
		}
	}
}

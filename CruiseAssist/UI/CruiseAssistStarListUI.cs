using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistStarListUI
	{
		public static float WindowWidth = 560f;
		public static float WindowHeight = 600f;
#if false
		public static float WindowWidth = 420f;
		public static float WindowHeight = 450f;
#endif
		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, WindowWidth, WindowHeight);
		public static int ListSelected;

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		private static Vector2 scrollPos = Vector2.zero;

		public static void OnGUI()
		{
			GUI.skin.window.fontSize = 11;

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
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			GUI.skin.button.alignment = TextAnchor.MiddleCenter;
			GUI.skin.button.fixedWidth = 120f;
			GUI.skin.button.fontSize = CruiseAssistMainUI.FontSize18;

			var style = new GUIStyle(GUI.skin.button);

			string[] texts = { "Normal", "History" };
			var selected = GUILayout.Toolbar(ListSelected, texts);
			if (selected != ListSelected)
			{
				ListSelected = selected;
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
			}

			GUILayout.EndHorizontal();

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.skin.label.fontSize = CruiseAssistMainUI.FontSize20;
			GUI.skin.button.fontSize = CruiseAssistMainUI.FontSize16;
			GUI.skin.button.fixedWidth = 0f;

			if (ListSelected == 0)
			{
				GameMain.galaxy.stars.Select(star => new Tuple<StarData, double>(star, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).OrderBy(tuple => tuple.v2).Do(tuple =>
				{
					var star = tuple.v1;
					var starName = CruiseAssist.GetStarName(star);
					if (GameMain.localStar != null && star.id == GameMain.localStar.id)
					{
						GameMain.localStar.planets.
							Select(planet => new Tuple<PlanetData, double>(planet, (planet.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
							AddItem(new Tuple<PlanetData, double>(null, (star.uPosition - GameMain.mainPlayer.uPosition).magnitude)).
							OrderBy(tuple2 => tuple2.v2).
							Do(tuple2 =>
							{
								var planet = tuple2.v1;
								GUILayout.BeginHorizontal();
								GUI.color = Color.white;
								if (planet == null)
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
									SelectStar(star, planet);
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
							SelectStar(star, null);
						}
						GUI.color = Color.white;
						GUILayout.EndHorizontal();
					}
				});
			}
			else if (ListSelected == 1)
			{
				bool highlighted = false;

				CruiseAssist.History.Reverse<int>().Do(id =>
				{
					var planet = GameMain.galaxy.PlanetById(id);
					var star = planet.star;
					var starName = CruiseAssist.GetStarName(star);
					var range = (planet.uPosition - GameMain.mainPlayer.uPosition).magnitude;
					GUILayout.BeginHorizontal();
					GUI.color = Color.white;
					if (!highlighted && CruiseAssist.SelectTargetPlanet != null && planet.id == CruiseAssist.SelectTargetPlanet.id)
					{
						GUI.color = Color.cyan;
						highlighted = true;
					}
					GUILayout.Label(starName + " - " + CruiseAssist.GetPlanetName(planet));
					GUILayout.FlexibleSpace();
					GUILayout.Label(CruiseAssistMainUI.RangeToString(range));
					GUI.color = Color.white;
					if (GUILayout.Button("SET"))
					{
						SelectStar(star, planet);
					}
					GUI.color = Color.white;
					GUILayout.EndHorizontal();
				});
			}

			GUILayout.EndScrollView();

			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Close", GUILayout.ExpandWidth(false)))
			{
				Show = false;
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

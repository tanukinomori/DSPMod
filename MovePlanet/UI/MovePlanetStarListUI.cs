using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace Tanukinomori
{
	public class MovePlanetStarListUI
	{
		public static float WindowWidth = 400f;
		public static float WindowHeight = 480f;

		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, WindowWidth, WindowHeight);
		public static int ListSelected = 0;

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		private static Vector2[] scrollPos = { Vector2.zero, Vector2.zero, Vector2.zero };

		private static StarData selectFromStar = null;
		private static PlanetData selectPlanet = null;

		public static void OnGUI()
		{
			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect = GUILayout.Window(99502252, Rect, WindowFunction, "MovePlanet - StarList", windowStyle);

			var scale = MovePlanetMainUI.Scale / 100.0f;

			if (Screen.width / scale < Rect.xMax)
			{
				Rect.x = Screen.width / scale - Rect.width;
			}
			if (Rect.x < 0)
			{
				Rect.x = 0;
			}

			if (Screen.height / scale < Rect.yMax)
			{
				Rect.y = Screen.height / scale - Rect.height;
			}
			if (Rect.y < 0)
			{
				Rect.y = 0;
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

			var mainWindowStyleButtonStyle = new GUIStyle(GUI.skin.button);
			mainWindowStyleButtonStyle.fixedWidth = 80;
			mainWindowStyleButtonStyle.fixedHeight = 20;
			mainWindowStyleButtonStyle.fontSize = 14;

			string[] texts = { "Planet", "System", "Mapping" };
			GUI.changed = false;
			var selected = GUILayout.Toolbar(ListSelected, texts, mainWindowStyleButtonStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
			}
			if (selected != ListSelected)
			{
				ListSelected = selected;
				nextCheckGameTick = GameMain.gameTick + 300;
			}

			var movePlanetTitleLabelStyle = new GUIStyle(GUI.skin.label);
			movePlanetTitleLabelStyle.fixedWidth = 100;
			movePlanetTitleLabelStyle.fixedHeight = 20;
			movePlanetTitleLabelStyle.fontSize = 14;
			movePlanetTitleLabelStyle.alignment = TextAnchor.UpperLeft;

			var nameLabelStyle = new GUIStyle(GUI.skin.label);
			nameLabelStyle.fixedWidth = 300;
			nameLabelStyle.stretchHeight = true;
			nameLabelStyle.fontSize = 14;
			nameLabelStyle.alignment = TextAnchor.MiddleLeft;

			GUILayout.EndHorizontal();

			if (ListSelected == 1)
			{
				GUILayout.BeginHorizontal();

				GUILayout.Label("Move Planet:", movePlanetTitleLabelStyle);

				if (selectPlanet != null)
				{
					GUILayout.Label(selectFromStar.displayName + " - " + selectPlanet.displayName, nameLabelStyle);
				}
				else
				{
					GUILayout.Label(" ", nameLabelStyle);
				}

				GUILayout.EndHorizontal();
			}

			scrollPos[ListSelected] = GUILayout.BeginScrollView(scrollPos[ListSelected]);

			var nActionButtonStyle = new GUIStyle(GUI.skin.button);
			nActionButtonStyle.fixedWidth = 40;
			nActionButtonStyle.fixedHeight = 18;
			nActionButtonStyle.margin.top = 6;
			nActionButtonStyle.fontSize = 12;
			var hActionButtonStyle = new GUIStyle(nActionButtonStyle);
			hActionButtonStyle.margin.top = 16;

			if (ListSelected == 0)
			{
				var selectedPlanetList = MovePlanet.PlanetStarMapping.Select(tuple => tuple.v1).ToList();

				GameMain.galaxy.stars.SelectMany(star => star.planets).Select(planet => new Tuple<int, PlanetData>(MovePlanet.GetOriginalId(planet.id), planet)).Where(tuple => tuple.v2.orbitAroundPlanet == null && !selectedPlanetList.Contains(tuple.v1)).OrderBy(tuple => tuple.v1).Do(tuple =>
				{
					GUILayout.BeginHorizontal();

					var originalId = tuple.v1;
					var planet = tuple.v2;
					var originalStar = GameMain.galaxy.StarById(originalId / 100);
					float textHeight;

					var satelliteCount = planet.star.planets.Where(p => p.orbitAroundPlanet != null && p.orbitAroundPlanet.id == planet.id).Count();

					var text = originalStar.displayName + " - " + planet.displayName + (satelliteCount > 0 ? $" (+{satelliteCount})" : "");
					GUILayout.Label(text, nameLabelStyle);
					textHeight = nameLabelStyle.CalcHeight(new GUIContent(text), nameLabelStyle.fixedWidth);

					GUILayout.FlexibleSpace();

					if (GUILayout.Button("SEL", textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
					{
						VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
						selectFromStar = originalStar;
						selectPlanet = planet;
						ListSelected = 1;
					}

					GUILayout.EndHorizontal();
				});
			}
			else if (ListSelected == 1)
			{
				GameMain.galaxy.stars.OrderBy(star => star.id).Do(star =>
				{
					GUILayout.BeginHorizontal();

					float textHeight;

					var text = star.displayName;
					GUILayout.Label(text, nameLabelStyle);
					textHeight = nameLabelStyle.CalcHeight(new GUIContent(text), nameLabelStyle.fixedWidth);

					GUILayout.FlexibleSpace();

					var canAdd = selectPlanet != null && star.id != selectFromStar.id;

					if (GUILayout.Button(canAdd ? "ADD" : "-", textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
					{
						VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
						if (canAdd)
						{
							MovePlanet.PlanetStarMapping.Add(new Tuple<int, int>(MovePlanet.GetOriginalId(selectPlanet.id), star.id));
							selectFromStar = null;
							selectPlanet = null;
							ListSelected = 2;
						}
					}

					GUILayout.EndHorizontal();
				});
			}
			else if (ListSelected == 2)
			{
				var listIndex = -1;

				MovePlanet.PlanetStarMapping.OrderBy(tuple => tuple.v1).Do(tuple =>
				{
					++listIndex;

					GUILayout.BeginHorizontal();

					var planetId = tuple.v1;
					var fromStarId = planetId / 100;
					var toStarId = tuple.v2;
					var planet = GameMain.galaxy.PlanetById(MovePlanet.GetNewId(planetId));
					var fromStar = GameMain.galaxy.StarById(fromStarId);
					var toStar = GameMain.galaxy.StarById(toStarId);
					float textHeight;

					var satelliteCount = planet.star.planets.Where(p => p.orbitAroundPlanet != null && p.orbitAroundPlanet.id == planet.id).Count();

					var text = fromStar.displayName + " - " + planet.displayName + (satelliteCount > 0 ? $" (+{satelliteCount})" : "") + " > " + toStar.displayName;
					GUILayout.Label(text, nameLabelStyle);
					textHeight = nameLabelStyle.CalcHeight(new GUIContent(text), nameLabelStyle.fixedWidth);

					GUILayout.FlexibleSpace();

					if (GUILayout.Button("DEL", textHeight < 30 ? nActionButtonStyle : hActionButtonStyle))
					{
						VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
						MovePlanet.PlanetStarMapping.RemoveAt(listIndex);
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

			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Close", buttonStyle))
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				Show = false;
			}

			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

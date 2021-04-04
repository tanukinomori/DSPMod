using UnityEngine;

namespace Tanukinomori
{
	public class MovePlanetMainUI
	{
		public static float Scale = 150.0f;

		public static float WindowWidth = 273f;
		public static float WindowHeight = 70f;

		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, WindowWidth, WindowHeight);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		public static void OnGUI()
		{
			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect = GUILayout.Window(99502251, Rect, WindowFunction, "MovePlanet", windowStyle);

			var scale = Scale / 100.0f;

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
			{
				var movePlanetAciviteLabelStyle = new GUIStyle(GUI.skin.label);
				movePlanetAciviteLabelStyle.fixedWidth = 145;
				movePlanetAciviteLabelStyle.fixedHeight = 32;
				movePlanetAciviteLabelStyle.fontSize = 14;
				movePlanetAciviteLabelStyle.alignment = TextAnchor.MiddleLeft;

				if (MovePlanet.SessionEnable)
				{
					movePlanetAciviteLabelStyle.normal.textColor = Color.cyan;
					GUILayout.Label("Move Planet Enabled.", movePlanetAciviteLabelStyle);
				}
				else
				{
					GUILayout.Label("Move Planet Disabled.", movePlanetAciviteLabelStyle);
				}

				GUILayout.FlexibleSpace();

				var buttonStyle = new GUIStyle(GUI.skin.button);
				buttonStyle.fixedWidth = 50;
				buttonStyle.fixedHeight = 18;
				buttonStyle.fontSize = 11;
				buttonStyle.alignment = TextAnchor.MiddleCenter;

				GUILayout.BeginVertical();

				if (GUILayout.Button("Config", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

					MovePlanetConfigUI.Show ^= true;
					if (MovePlanetConfigUI.Show)
					{
						MovePlanetConfigUI.TempScale = MovePlanetMainUI.Scale;
					}
				}

				if (GUILayout.Button(MovePlanet.ConfigEnable ? "Enable" : "Disable", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
					MovePlanet.ConfigEnable ^= true;
					nextCheckGameTick = GameMain.gameTick + 300;
				}

				GUILayout.EndVertical();

				GUILayout.BeginVertical();

				if (GUILayout.Button("StarList", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
					MovePlanetStarListUI.Show ^= true;
				}

				if (GUILayout.Button("-", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				}

				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

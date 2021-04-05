using System.Linq;
using UnityEngine;

namespace Tanukinomori
{
	public class MovePlanetDebugUI
	{
		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, 400f, 400f);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		private static Vector2 scrollPos = Vector2.zero;

		public static void OnGUI()
		{
			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect = GUILayout.Window(99502254, Rect, WindowFunction, "MovePlanet - Debug", windowStyle);

			var scale = MovePlanetMainUI.Scale / 100.0f;

			if (Screen.width < Rect.xMax)
			{
				Rect.x = Screen.width - Rect.width;
			}
			if (Rect.x < 0)
			{
				Rect.x = 0;
			}

			if (Screen.height < Rect.yMax)
			{
				Rect.y = Screen.height - Rect.height;
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

			var labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.fontSize = 12;

			scrollPos = GUILayout.BeginScrollView(scrollPos);

			GUILayout.Label($"DSPGame.IsMenuDemo={DSPGame.IsMenuDemo}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer={GameMain.mainPlayer}", labelStyle);

			var uiGame = UIRoot.instance?.uiGame;
			if (uiGame != null)
			{

//				GUILayout.Label($"uiGame.guideComplete={uiGame.guideComplete}", labelStyle);

				GUILayout.Label($"uiGame.techTree.active={uiGame.techTree.active}", labelStyle);
				GUILayout.Label($"uiGame.escMenu.active={uiGame.escMenu.active}", labelStyle);
				GUILayout.Label($"uiGame.dysonmap.active={uiGame.dysonmap.active}", labelStyle);

				GUILayout.Label($"uiGame.hideAllUI0={uiGame.hideAllUI0}", labelStyle);
				GUILayout.Label($"uiGame.hideAllUI1={uiGame.hideAllUI1}", labelStyle);

				GUILayout.Label($"uiGame.starmap.active={uiGame.starmap.active}", labelStyle);
			}
			GUILayout.Label($"LoadGameWindowActive={MovePlanet.LoadGameWindowActive}", labelStyle);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

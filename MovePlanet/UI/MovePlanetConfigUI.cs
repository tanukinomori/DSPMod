using UnityEngine;

namespace Tanukinomori
{
	public class MovePlanetConfigUI
	{
		private static int wIdx = 0;

		public static float WindowWidth = 400f;
		public static float WindowHeight = 480f;

		public static bool[] Show = { false, false };
		public static Rect[] Rect = {
			new Rect(0f, 0f, WindowWidth, WindowHeight),
			new Rect(0f, 0f, WindowWidth, WindowHeight) };

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		public static long NextCheckGameTick = long.MaxValue;

		public static float TempScale = 150.0f;

		public static void OnGUI()
		{
			wIdx = MovePlanetMainUI.wIdx;

			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect[wIdx] = GUILayout.Window(99502253, Rect[wIdx], WindowFunction, "MovePlanet - Config", windowStyle);

			var scale = MovePlanetMainUI.Scale / 100.0f;

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
					NextCheckGameTick = GameMain.gameTick + 300;
				}
			}

			lastCheckWindowLeft = Rect[wIdx].x;
			lastCheckWindowTop = Rect[wIdx].y;

			if (NextCheckGameTick <= GameMain.gameTick)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);
				NextCheckGameTick = long.MaxValue;
			}
		}

		public static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			var uiScaleTitleLabelStyle = new GUIStyle(GUI.skin.label);
			uiScaleTitleLabelStyle.fixedWidth = 60;
			uiScaleTitleLabelStyle.fixedHeight = 20;
			uiScaleTitleLabelStyle.fontSize = 12;
			uiScaleTitleLabelStyle.alignment = TextAnchor.MiddleLeft;

			GUILayout.Label("UI Scale :", uiScaleTitleLabelStyle);

			var scaleSliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
			scaleSliderStyle.fixedWidth = 180;
			scaleSliderStyle.margin.top = 10;
			scaleSliderStyle.alignment = TextAnchor.MiddleLeft;
			var scaleSliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);

			TempScale = GUILayout.HorizontalSlider(TempScale, 80.0f, 240.0f, scaleSliderStyle, scaleSliderThumbStyle);

			TempScale = (int)TempScale / 5 * 5;

			var uiScaleValueLabelStyle = new GUIStyle(GUI.skin.label);
			uiScaleValueLabelStyle.fixedWidth = 40;
			uiScaleValueLabelStyle.fixedHeight = 20;
			uiScaleValueLabelStyle.fontSize = 12;
			uiScaleValueLabelStyle.alignment = TextAnchor.MiddleLeft;

			GUILayout.Label(TempScale.ToString("0") + "%", uiScaleValueLabelStyle);

			var scaleSetButtonStyle = new GUIStyle(GUI.skin.button);
			scaleSetButtonStyle.fixedWidth = 60;
			scaleSetButtonStyle.fixedHeight = 18;
			scaleSetButtonStyle.margin.top = 6;
			scaleSetButtonStyle.fontSize = 12;

			if (GUILayout.Button("SET", scaleSetButtonStyle))
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				MovePlanetMainUI.Scale = TempScale;
				NextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.EndHorizontal();

			var toggleStyle = new GUIStyle(GUI.skin.toggle);
			toggleStyle.fixedHeight = 20;
			toggleStyle.fontSize = 12;

			GUI.changed = false;
			MovePlanet.LoadWarperFlag = GUILayout.Toggle(MovePlanet.LoadWarperFlag, "Load the warper onto a moving Logistics vessel during a Load Game.", toggleStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				NextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.FlexibleSpace();

			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			var closeButtonStyle = new GUIStyle(GUI.skin.button);
			closeButtonStyle.fixedWidth = 80;
			closeButtonStyle.fixedHeight = 20;
			closeButtonStyle.fontSize = 12;

			if (GUILayout.Button("Close", closeButtonStyle))
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				Show[MovePlanetMainUI.wIdx] = false;
			}

			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

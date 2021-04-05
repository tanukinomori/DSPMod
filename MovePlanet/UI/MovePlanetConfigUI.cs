using UnityEngine;

namespace Tanukinomori
{
	public class MovePlanetConfigUI
	{
		public static float WindowWidth = 400f;
		public static float WindowHeight = 480f;

		public static bool[] Show = { false, false };
		public static Rect Rect = new Rect(0f, 0f, WindowWidth, WindowHeight);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		public static float TempScale = 150.0f;

		public static void OnGUI()
		{
			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect = GUILayout.Window(99502253, Rect, WindowFunction, "MovePlanet - Config", windowStyle);

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
				nextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.EndVertical();

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

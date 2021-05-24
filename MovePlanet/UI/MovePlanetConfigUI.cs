using UnityEngine;

namespace tanu.MovePlanet
{
	public class MovePlanetConfigUI
	{
		private static int wIdx = 0;

		public const float WindowWidth = 400f;
		public const float WindowHeight = 300f;

		public static bool[] Show = { false, false };
		public static Rect[] Rect = {
			new Rect(0f, 0f, WindowWidth, WindowHeight),
			new Rect(0f, 0f, WindowWidth, WindowHeight) };

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;

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
					MovePlanetMainUI.NextCheckGameTick = GameMain.gameTick + 300;
				}
			}

			lastCheckWindowLeft = Rect[wIdx].x;
			lastCheckWindowTop = Rect[wIdx].y;
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
				MovePlanetMainUI.NextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.EndHorizontal();

			var nToggleStyle = new GUIStyle(GUI.skin.toggle);
			nToggleStyle.fixedHeight = 16;
			nToggleStyle.fontSize = 12;
			var hToggleStyle = new GUIStyle(nToggleStyle);
			hToggleStyle.fixedHeight = 32;

			GUI.changed = false;
			MovePlanet.MovePlayerFlag = GUILayout.Toggle(MovePlanet.MovePlayerFlag, " If player in space when Load Game,\n move to first planet.", hToggleStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				MovePlanetMainUI.NextCheckGameTick = GameMain.gameTick + 300;
			}

			GUI.changed = false;
			MovePlanet.LoadWarperFlag = GUILayout.Toggle(MovePlanet.LoadWarperFlag, " Warp the Logistics vessel when Load Game.", nToggleStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				MovePlanetMainUI.NextCheckGameTick = GameMain.gameTick + 300;
			}

			GUI.changed = false;
			MovePlanet.MarkVisitedFlag = GUILayout.Toggle(MovePlanet.MarkVisitedFlag, "Mark the visited system and planet.", nToggleStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
				MovePlanetMainUI.NextCheckGameTick = GameMain.gameTick + 300;
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

using System;
using UnityEngine;

namespace Tanukinomori.UI
{
	public class CruiseAssistConfigUI
	{
		private static int wIdx = 0;

		public static float WindowWidth = 400f;
		public static float WindowHeight = 400f;

		public static bool[] Show = { false, false };
		public static Rect[] Rect = {
			new Rect(0f, 0f, WindowWidth, WindowHeight),
			new Rect(0f, 0f, WindowWidth, WindowHeight) };

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		public static float TempScale = 100.0f;

		public static void OnGUI()
		{
			wIdx = CruiseAssistMainUI.wIdx;

			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect[wIdx] = GUILayout.Window(99030293, Rect[wIdx], WindowFunction, "CruiseAssist - Config", windowStyle);

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

			var mainWindowStyleLabelStyle = new GUIStyle(GUI.skin.label);
			mainWindowStyleLabelStyle.fixedWidth = 120;
			mainWindowStyleLabelStyle.fixedHeight = 20;
			mainWindowStyleLabelStyle.fontSize = 12;
			mainWindowStyleLabelStyle.alignment = TextAnchor.MiddleLeft;

			GUILayout.Label("Main Window Style :", mainWindowStyleLabelStyle);

			var mainWindowStyleButtonStyle = new GUIStyle(GUI.skin.button);
			mainWindowStyleButtonStyle.fixedWidth = 80;
			mainWindowStyleButtonStyle.fixedHeight = 20;
			mainWindowStyleButtonStyle.fontSize = 12;

			string[] texts = { "FULL", "MINI" };
			int listSelected =
				CruiseAssistMainUI.ViewMode == CruiseAssistMainUIViewMode.FULL ? 0 : 1;
			GUI.changed = false;
			var selected = GUILayout.Toolbar(listSelected, texts, mainWindowStyleButtonStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
			}
			if (selected != listSelected)
			{
				switch (selected)
				{
					case 0:
						CruiseAssistMainUI.ViewMode = CruiseAssistMainUIViewMode.FULL;
						break;
					case 1:
						CruiseAssistMainUI.ViewMode = CruiseAssistMainUIViewMode.MINI;
						break;
				}
				nextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.EndHorizontal();

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
				CruiseAssistMainUI.Scale = TempScale;
				nextCheckGameTick = GameMain.gameTick + 300;
			}

			GUILayout.EndHorizontal();

			var toggleStyle = new GUIStyle(GUI.skin.toggle);
			toggleStyle.fixedHeight = 20;
			toggleStyle.fontSize = 12;

			GUI.changed = false;
			CruiseAssist.SelectFocusFlag = GUILayout.Toggle(CruiseAssist.SelectFocusFlag, "Focus when target selected.", toggleStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
			}

			GUI.changed = false;
			CruiseAssist.HideDuplicateHistoryFlag = GUILayout.Toggle(CruiseAssist.HideDuplicateHistoryFlag, "Hide duplicate history.", toggleStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
			}

			GUI.changed = false;
			CruiseAssist.AutoDisableLockCursorFlag = GUILayout.Toggle(CruiseAssist.AutoDisableLockCursorFlag, "Disable lock cursor when starting sail mode.", toggleStyle);
			if (GUI.changed)
			{
				VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
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
				Show[wIdx] = false;
			}

			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

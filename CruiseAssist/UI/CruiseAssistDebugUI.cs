using System.Linq;
using Tanukinomori.commons;
using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistDebugUI
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

			Rect = GUILayout.Window(99030294, Rect, WindowFunction, "CruiseAssist - Debug", windowStyle);

			var scale = CruiseAssistMainUI.Scale / 100.0f;

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

			GUILayout.Label($"CruiseAssist.ReticuleTargetStar.id={CruiseAssist.ReticuleTargetStar?.id}", labelStyle);
			GUILayout.Label($"CruiseAssist.ReticuleTargetPlanet.id={CruiseAssist.ReticuleTargetPlanet?.id}", labelStyle);
			GUILayout.Label($"CruiseAssist.SelectTargetStar.id={CruiseAssist.SelectTargetStar?.id}", labelStyle);
			GUILayout.Label($"CruiseAssist.SelectTargetPlanet.id={CruiseAssist.SelectTargetPlanet?.id}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.navigation.indicatorAstroId={GameMain.mainPlayer.navigation.indicatorAstroId}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.w={GameMain.mainPlayer.controller.input0.w}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.x={GameMain.mainPlayer.controller.input0.x}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.y={GameMain.mainPlayer.controller.input0.y}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.z={GameMain.mainPlayer.controller.input0.z}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.w={GameMain.mainPlayer.controller.input1.w}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.x={GameMain.mainPlayer.controller.input1.x}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.y={GameMain.mainPlayer.controller.input1.y}", labelStyle);
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.z={GameMain.mainPlayer.controller.input1.z}", labelStyle);
			GUILayout.Label($"VFInput._sailSpeedUp={VFInput._sailSpeedUp}", labelStyle);
			GUILayout.Label($"CruiseAssist.Enable={CruiseAssist.Enable}", labelStyle);
			GUILayout.Label($"CruiseAssist.History={CruiseAssist.History.Count()}", labelStyle);
			GUILayout.Label($"CruiseAssist.History={ListUtils.ToString(CruiseAssist.History)}", labelStyle);
			GUILayout.Label($"GUI.skin.window.margin.top={GUI.skin.window.margin.top}", labelStyle);
			GUILayout.Label($"GUI.skin.window.border.top={GUI.skin.window.border.top}", labelStyle);
			GUILayout.Label($"GUI.skin.window.padding.top={GUI.skin.window.padding.top}", labelStyle);
			GUILayout.Label($"GUI.skin.window.overflow.top={GUI.skin.window.overflow.top}", labelStyle);

			GUILayout.EndScrollView();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

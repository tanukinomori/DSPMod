using System.Linq;
using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistDebugUI
	{
		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, 600f, 200f);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		public static void OnGUI()
		{
			GUI.skin.window.fontSize = 11;

			Rect = GUILayout.Window(99030293, Rect, WindowFunction, "CruiseAssist - Debug");

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

			GUI.skin.label.fontSize = 16;

			GUILayout.Label($"CruiseAssist.ReticuleTargetStar.id={CruiseAssist.ReticuleTargetStar?.id}");
			GUILayout.Label($"CruiseAssist.ReticuleTargetPlanet.id={CruiseAssist.ReticuleTargetPlanet?.id}");
			GUILayout.Label($"CruiseAssist.SelectTargetStar.id={CruiseAssist.SelectTargetStar?.id}");
			GUILayout.Label($"CruiseAssist.SelectTargetPlanet.id={CruiseAssist.SelectTargetPlanet?.id}");
			GUILayout.Label($"GameMain.mainPlayer.navigation.indicatorAstroId={GameMain.mainPlayer.navigation.indicatorAstroId}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.w={GameMain.mainPlayer.controller.input0.w}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.x={GameMain.mainPlayer.controller.input0.x}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.y={GameMain.mainPlayer.controller.input0.y}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input0.z={GameMain.mainPlayer.controller.input0.z}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.w={GameMain.mainPlayer.controller.input1.w}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.x={GameMain.mainPlayer.controller.input1.x}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.y={GameMain.mainPlayer.controller.input1.y}");
			GUILayout.Label($"GameMain.mainPlayer.controller.input1.z={GameMain.mainPlayer.controller.input1.z}");
			GUILayout.Label($"VFInput._sailSpeedUp={VFInput._sailSpeedUp}");
			GUILayout.Label($"CruiseAssist.Enable={CruiseAssist.Enable}");
			GUILayout.Label($"CruiseAssist.History={CruiseAssist.History.Count()}");
			GUILayout.Label("CruiseAssist.History=" + (CruiseAssist.History.Count > 0 ? CruiseAssist.History.Select(id => id.ToString()).Aggregate((a, b) => a + "," + b) : ""));

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

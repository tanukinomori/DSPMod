using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistUI
	{
		public static bool Show = false;
		public static Rect Rect = new Rect(100f, 100f, 480f, 120f);
		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		public static void OnGUI()
		{
			Rect = GUILayout.Window(99030291, Rect, WindowFunction, "CruiseAssist".Translate());
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

		private static void WindowFunction(int windowId)
		{
			GUI.skin.label.fontSize = 20;
			GUI.skin.label.alignment = TextAnchor.MiddleLeft;
			GUI.color = Color.white;

			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Target System: ".Translate());
				if (CruiseAssist.TargetStar != null)
				{
					if (CruiseAssist.State == CruiseAssistState.TO_STAR_CURSOR)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label(CruiseAssist.TargetStar.name);
					GUI.color = Color.white;
				}
				GUILayout.FlexibleSpace();
				//GUILayout.Button("S");
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Target Planet: ".Translate());
				if (CruiseAssist.TargetPlanet != null)
				{
					if (CruiseAssist.State == CruiseAssistState.TO_PLANET_CURSOR)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label(CruiseAssist.TargetPlanet.name);
					GUI.color = Color.white;
				}
				GUILayout.FlexibleSpace();
				//GUILayout.Button("S");
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (CruiseAssist.State == CruiseAssistState.INACTIVE)
			{
				GUILayout.Label("Cruise Assist Inactivated.".Translate());
			}
			else
			{
				GUI.color = Color.cyan;
				GUILayout.Label("Cruise Assist Activated.".Translate());
				GUI.color = Color.white;
			}
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}
	}
}

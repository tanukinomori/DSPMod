using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistUI
	{
		public static bool Show = false;
		public static Rect Rect = new Rect(100f, 100f, 540f, 120f);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		public static void OnGUI()
		{
			Rect = GUILayout.Window(99030291, Rect, WindowFunction, "CruiseAssist".Translate());

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

		private static void WindowFunction(int windowId)
		{
			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical();
			{
				GUI.color = Color.white;
				GUI.skin.label.alignment = TextAnchor.UpperLeft;
				GUI.skin.label.fontSize = 16;

				GUILayout.Label("Target System:".Translate());
				GUILayout.Label("Target Planet:".Translate());
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical();
			{
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				GUI.skin.label.fontSize = 20;

				if (CruiseAssist.TargetStar != null)
				{
					if (CruiseAssist.State == CruiseAssistState.TO_STAR_CURSOR)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label(CruiseAssist.TargetStar.name);
					GUI.color = Color.white;
				}
				else
				{
					GUILayout.Label(" ");
				}

				if (CruiseAssist.TargetPlanet != null)
				{
					if (CruiseAssist.State == CruiseAssistState.TO_PLANET_CURSOR)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label(CruiseAssist.TargetPlanet.name);
					GUI.color = Color.white;
				}
				else
				{
					GUILayout.Label(" ");
				}
			}
			GUILayout.EndVertical();

			GUILayout.FlexibleSpace();

			GUILayout.BeginVertical();
			{
				GUI.skin.label.alignment = TextAnchor.LowerLeft;
				GUI.skin.label.fontSize = 16;
				if (CruiseAssist.TargetStar != null)
				{
					if (CruiseAssist.State == CruiseAssistState.TO_STAR_CURSOR)
					{
						GUI.color = Color.cyan;
					}
					double range = (CruiseAssist.TargetStar.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)(CruiseAssist.TargetStar.viewRadius - 120f);
					GUILayout.Label(CruiseAssistUI.RangeToString(range));
					GUI.color = Color.white;
				}
				else
				{
					GUILayout.Label(" ");
				}
				if (CruiseAssist.TargetPlanet != null)
				{
					if (CruiseAssist.State == CruiseAssistState.TO_PLANET_CURSOR)
					{
						GUI.color = Color.cyan;
					}
					double range = (CruiseAssist.TargetPlanet.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)CruiseAssist.TargetPlanet.realRadius;
					GUILayout.Label(CruiseAssistUI.RangeToString(range));
					GUI.color = Color.white;
				}
				else
				{
					GUILayout.Label(" ");
				}
			}
			GUILayout.EndVertical();

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUI.skin.label.fontSize = 20;

				if (CruiseAssist.State == CruiseAssistState.INACTIVE)
				{
					GUI.color = Color.white;
					GUILayout.Label("Cruise Assist Inactivated.".Translate());
				}
				else
				{
					GUI.color = Color.cyan;
					GUILayout.Label("Cruise Assist Activated.".Translate());
					GUI.color = Color.white;
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		private static string RangeToString(double range)
		{
			if (range < 10000.0)
			{
				return ((int)(range + 0.5)).ToString() + "m";
			}
			else
				if (range < 600000.0)
			{
				return (range / 40000.0).ToString("0.00") + "AU";
			}
			else
			{
				return (range / 2400000.0).ToString("0.00") + "Ly";
			}
		}
	}
}

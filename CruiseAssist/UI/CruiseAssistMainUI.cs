using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistMainUI
	{
		public static CruiseAssistMainUIViewMode ViewMode = CruiseAssistMainUIViewMode.FULL;

		public static int FontSize14 = 14;
		public static int FontSize16 = 16;
		public static int FontSize18 = 18;
		public static int FontSize20 = 20;
		public static float WindowWidthFull = 560f;
		public static float WindowHeightFull = 200f;
		public static float WindowWidthMini = 360f;
		public static float WindowHeightMini = 76f;
#if false
		public static int FontSize16 = 14;
		public static int FontSize20 = 15;
		public static float WindowWidth = 420f;
		public static float WindowHeight = 90f;
#endif
		public static bool Show = false;
		public static Rect Rect = new Rect(0f, 0f, WindowWidthFull, WindowHeightFull);

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;
		private static long nextCheckGameTick = long.MaxValue;

		public static void OnGUI()
		{
			switch (ViewMode)
			{
				case CruiseAssistMainUIViewMode.FULL:
					Rect.width = WindowWidthFull;
					Rect.height = WindowHeightFull;
					break;
				case CruiseAssistMainUIViewMode.MINI:
					Rect.width = WindowWidthMini;
					Rect.height = WindowHeightMini;
					break;
			}

			GUI.skin.window.fontSize = 11;

			Rect = GUILayout.Window(99030291, Rect, WindowFunction, "CruiseAssist");

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

			if (ViewMode == CruiseAssistMainUIViewMode.FULL)
			{
				GUILayout.BeginHorizontal();

				GUILayout.BeginVertical();
				{
					GUI.color = Color.white;
					GUI.skin.label.alignment = TextAnchor.UpperLeft;
					GUI.skin.label.fontSize = FontSize16;

					if (CruiseAssist.State == CruiseAssistState.TO_STAR)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label("Target\n System:", GUILayout.ExpandHeight(true));
					GUI.color = Color.white;

					if (CruiseAssist.State == CruiseAssistState.TO_PLANET)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label("Target\n Planet:", GUILayout.ExpandHeight(true));
					GUI.color = Color.white;
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					GUI.skin.label.alignment = TextAnchor.MiddleLeft;
					GUI.skin.label.fontSize = FontSize20;

					if (CruiseAssist.TargetStar != null)
					{
						if (CruiseAssist.State == CruiseAssistState.TO_STAR)
						{
							GUI.color = Color.cyan;
						}
						GUILayout.Label(CruiseAssist.GetStarName(CruiseAssist.TargetStar), GUILayout.ExpandHeight(true));
						GUI.color = Color.white;
					}
					else
					{
						GUILayout.Label(" ", GUILayout.ExpandHeight(true));
					}

					if (CruiseAssist.TargetPlanet != null)
					{
						if (CruiseAssist.State == CruiseAssistState.TO_PLANET)
						{
							GUI.color = Color.cyan;
						}
						GUILayout.Label(CruiseAssist.GetPlanetName(CruiseAssist.TargetPlanet), GUILayout.ExpandHeight(true));
						GUI.color = Color.white;
					}
					else
					{
						GUILayout.Label(" ", GUILayout.ExpandHeight(true));
					}
				}
				GUILayout.EndVertical();

				GUILayout.FlexibleSpace();

				GUILayout.BeginVertical();
				{
					var actionSail = GameMain.mainPlayer.controller.actionSail;
					var visual_uvel = actionSail.visual_uvel;
					double velocity;
					if (GameMain.mainPlayer.warping)
					{
						velocity = (visual_uvel + actionSail.currentWarpVelocity).magnitude;
					}
					else
					{
						velocity = visual_uvel.magnitude;
					}

					GUI.skin.label.alignment = TextAnchor.MiddleRight;
					GUI.skin.label.fontSize = FontSize16;
					if (CruiseAssist.TargetStar != null)
					{
						if (CruiseAssist.State == CruiseAssistState.TO_STAR)
						{
							GUI.color = Color.cyan;
						}
						var range = (CruiseAssist.TargetStar.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)(CruiseAssist.TargetStar.viewRadius - 120f);
						GUILayout.Label(RangeToString(range) + "\n" + TimeToString(range / velocity), GUILayout.ExpandHeight(true));
						GUI.color = Color.white;
					}
					else
					{
						GUILayout.Label(" \n ", GUILayout.ExpandHeight(true));
					}
					if (CruiseAssist.TargetPlanet != null)
					{
						if (CruiseAssist.State == CruiseAssistState.TO_PLANET)
						{
							GUI.color = Color.cyan;
						}
						var range = (CruiseAssist.TargetPlanet.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)CruiseAssist.TargetPlanet.realRadius;
						GUILayout.Label(RangeToString(range) + "\n" + TimeToString(range / velocity), GUILayout.ExpandHeight(true));
						GUI.color = Color.white;
					}
					else
					{
						GUILayout.Label(" \n ", GUILayout.ExpandHeight(true));
					}
				}
				GUILayout.EndVertical();

				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			{
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				GUI.skin.label.fontSize = FontSize20;

				if (CruiseAssist.State == CruiseAssistState.INACTIVE)
				{
					GUI.color = Color.white;
					GUILayout.Label("Cruise Assist Inactive.", GUILayout.ExpandHeight(true));
				}
				else
				{
					GUI.color = Color.cyan;
					GUILayout.Label("Cruise Assist Active.", GUILayout.ExpandHeight(true));
					GUI.color = Color.white;
				}

				GUILayout.FlexibleSpace();

				GUI.skin.button.alignment = TextAnchor.MiddleCenter;
				GUI.skin.button.fixedWidth = 60f;
				GUI.skin.button.fontSize = CruiseAssistMainUI.FontSize14;

				GUILayout.BeginVertical();

				if (GUILayout.Button(ViewMode.ToString()))
				{
					switch (ViewMode)
					{
						case CruiseAssistMainUIViewMode.FULL:
							ViewMode = CruiseAssistMainUIViewMode.MINI;
							break;
						case CruiseAssistMainUIViewMode.MINI:
							ViewMode = CruiseAssistMainUIViewMode.FULL;
							break;
					}
					nextCheckGameTick = GameMain.gameTick + 300;
				}

				if (GUILayout.Button(CruiseAssist.Enable ? "Enable" : "Disable"))
				{
					CruiseAssist.Enable = !CruiseAssist.Enable;
					nextCheckGameTick = GameMain.gameTick + 300;
				}

				GUILayout.EndVertical();

				GUILayout.BeginVertical();

				if (GUILayout.Button("StarList"))
				{
					CruiseAssistStarListUI.Show = !CruiseAssistStarListUI.Show;
				}

				if (GUILayout.Button("Cancel"))
				{
					CruiseAssistStarListUI.SelectStar(null, null);
				}

				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		public static string RangeToString(double range)
		{
			if (range < 10000.0)
			{
				return ((int)(range + 0.5)).ToString() + "m ";
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

		public static string TimeToString(double time)
		{
			return time.ToString("0.0") + "s ";
		}
	}
}

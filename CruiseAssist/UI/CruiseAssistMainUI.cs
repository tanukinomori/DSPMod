using UnityEngine;

namespace Tanukinomori
{
	public class CruiseAssistMainUI
	{
		public static float Scale = 150.0f;

		public static int wIdx = 0;

		public static CruiseAssistMainUIViewMode ViewMode = CruiseAssistMainUIViewMode.FULL;

		public const float WindowWidthFull = 398f;
		public const float WindowHeightFull = 150f;
		public const float WindowWidthMini = 273f;
		public const float WindowHeightMini = 70f;

		public static Rect[] Rect = {
			new Rect(0f, 0f, WindowWidthFull, WindowHeightFull),
			new Rect(0f, 0f, WindowWidthFull, WindowHeightFull) };

		private static float lastCheckWindowLeft = float.MinValue;
		private static float lastCheckWindowTop = float.MinValue;

		public static long NextCheckGameTick = long.MaxValue;

		public static void OnGUI()
		{
			switch (ViewMode)
			{
				case CruiseAssistMainUIViewMode.FULL:
					Rect[wIdx].width = WindowWidthFull;
					Rect[wIdx].height = WindowHeightFull;
					break;
				case CruiseAssistMainUIViewMode.MINI:
					Rect[wIdx].width = WindowWidthMini;
					Rect[wIdx].height = WindowHeightMini;
					break;
			}

			var windowStyle = new GUIStyle(GUI.skin.window);
			windowStyle.fontSize = 11;

			Rect[wIdx] = GUILayout.Window(99030291, Rect[wIdx], WindowFunction, "CruiseAssist", windowStyle);

			//LogManager.LogInfo($"Rect[wIdx].width={Rect[wIdx].width}, Rect[wIdx].height={Rect[wIdx].height}");

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

			if (ViewMode == CruiseAssistMainUIViewMode.FULL)
			{
				GUILayout.BeginHorizontal();

				Color systemTextColor =
					CruiseAssist.State == CruiseAssistState.TO_STAR ? Color.cyan : Color.white;
				Color planetTextColor =
					CruiseAssist.State == CruiseAssistState.TO_PLANET ? Color.cyan : Color.white;

				GUILayout.BeginVertical();
				{
					var targetSystemTitleLabelStyle = new GUIStyle(GUI.skin.label);
					targetSystemTitleLabelStyle.fixedWidth = 50;
					targetSystemTitleLabelStyle.fixedHeight = 36;
					targetSystemTitleLabelStyle.fontSize = 12;
					targetSystemTitleLabelStyle.alignment = TextAnchor.UpperLeft;
					var targetPlanetTitleLabelStyle = new GUIStyle(targetSystemTitleLabelStyle);

					targetSystemTitleLabelStyle.normal.textColor = systemTextColor;

					GUILayout.Label("Target\n System:", targetSystemTitleLabelStyle);

					targetPlanetTitleLabelStyle.normal.textColor = planetTextColor;

					GUILayout.Label("Target\n Planet:", targetPlanetTitleLabelStyle);
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					var targetSystemNameLabelStyle = new GUIStyle(GUI.skin.label);
					targetSystemNameLabelStyle.fixedWidth = 240;
					targetSystemNameLabelStyle.fixedHeight = 36;
					targetSystemNameLabelStyle.fontSize = 14;
					targetSystemNameLabelStyle.alignment = TextAnchor.MiddleLeft;
					var targetPlanetNameLabelStyle = new GUIStyle(targetSystemNameLabelStyle);

					if (CruiseAssist.TargetStar != null)
					{
						targetSystemNameLabelStyle.normal.textColor = systemTextColor;

						GUILayout.Label(CruiseAssist.GetStarName(CruiseAssist.TargetStar), targetSystemNameLabelStyle);
					}
					else
					{
						GUILayout.Label(" ", targetSystemNameLabelStyle);
					}

					if (CruiseAssist.TargetPlanet != null)
					{
						targetPlanetNameLabelStyle.normal.textColor = planetTextColor;

						GUILayout.Label(CruiseAssist.GetPlanetName(CruiseAssist.TargetPlanet), targetPlanetNameLabelStyle);
					}
					else
					{
						GUILayout.Label(" ", targetPlanetNameLabelStyle);
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

					var targetSystemRangeTimeLabelStyle = new GUIStyle(GUI.skin.label);
					targetSystemRangeTimeLabelStyle.fixedWidth = 80;
					targetSystemRangeTimeLabelStyle.fixedHeight = 36;
					targetSystemRangeTimeLabelStyle.fontSize = 12;
					targetSystemRangeTimeLabelStyle.alignment = TextAnchor.MiddleRight;
					var targetPlanetRangeTimeLabelStyle = new GUIStyle(targetSystemRangeTimeLabelStyle);

					if (CruiseAssist.TargetStar != null)
					{
						targetSystemRangeTimeLabelStyle.normal.textColor = systemTextColor;

						var range = (CruiseAssist.TargetStar.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)(CruiseAssist.TargetStar.viewRadius - 120f);
						GUILayout.Label(RangeToString(range) + "\n" + TimeToString(range / velocity), targetSystemRangeTimeLabelStyle);
					}
					else
					{
						GUILayout.Label(" \n ", targetSystemRangeTimeLabelStyle);
					}
					if (CruiseAssist.TargetPlanet != null)
					{
						targetPlanetRangeTimeLabelStyle.normal.textColor = planetTextColor;

						var range = (CruiseAssist.TargetPlanet.uPosition - GameMain.mainPlayer.uPosition).magnitude - (double)CruiseAssist.TargetPlanet.realRadius;
						GUILayout.Label(RangeToString(range) + "\n" + TimeToString(range / velocity), targetPlanetRangeTimeLabelStyle);
					}
					else
					{
						GUILayout.Label(" \n ", targetPlanetRangeTimeLabelStyle);
					}
				}
				GUILayout.EndVertical();

				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			{
				var cruiseAssistAciviteLabelStyle = new GUIStyle(GUI.skin.label);
				cruiseAssistAciviteLabelStyle.fixedWidth = 145;
				cruiseAssistAciviteLabelStyle.fixedHeight = 32;
				cruiseAssistAciviteLabelStyle.fontSize = 14;
				cruiseAssistAciviteLabelStyle.alignment = TextAnchor.MiddleLeft;

				if (CruiseAssist.State == CruiseAssistState.INACTIVE)
				{
					GUILayout.Label("Cruise Assist Inactive.", cruiseAssistAciviteLabelStyle);
				}
				else
				{
					cruiseAssistAciviteLabelStyle.normal.textColor = Color.cyan;
					GUILayout.Label("Cruise Assist Active.", cruiseAssistAciviteLabelStyle);
				}

				GUILayout.FlexibleSpace();

				var buttonStyle = new GUIStyle(GUI.skin.button);
				buttonStyle.fixedWidth = 50;
				buttonStyle.fixedHeight = 18;
				buttonStyle.fontSize = 11;
				buttonStyle.alignment = TextAnchor.MiddleCenter;

				GUILayout.BeginVertical();

				if (GUILayout.Button("Config", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);

					CruiseAssistConfigUI.Show[wIdx] ^= true;
					if (CruiseAssistConfigUI.Show[wIdx])
					{
						CruiseAssistConfigUI.TempScale = CruiseAssistMainUI.Scale;
					}
				}

				if (GUILayout.Button(CruiseAssist.Enable ? "Enable" : "Disable", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
					CruiseAssist.Enable ^= true;
					NextCheckGameTick = GameMain.gameTick + 300;
				}

				GUILayout.EndVertical();

				GUILayout.BeginVertical();

				if (GUILayout.Button("StarList", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
					CruiseAssistStarListUI.Show[wIdx] ^= true;
				}

				if (GUILayout.Button("Cancel", buttonStyle))
				{
					VFAudio.Create("ui-click-0", null, Vector3.zero, true, 0);
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
			int s = (int)(time + 0.5);
			int m = s / 60;
			int h = m / 60;
			s %= 60;
			m %= 60;
			return string.Format("{0:00} {1:00} {2:00}", h, m, s);
		}
	}
}

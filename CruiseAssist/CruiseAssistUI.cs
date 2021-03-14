using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Tanukinomori
{
	public class CruiseAssistUI
	{
		public static bool Show = false;

		private static Rect rect = new Rect(100f, 100f, 400f, 120f);

		public static void OnGUI()
		{
			rect = GUILayout.Window(99030291, rect, WindowFunction, "CruiseAssist".Translate());
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
				if (CruiseAssist.targetStar != null)
				{
					if (CruiseAssist.state == CruiseAssist.State.TO_STAR)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label(CruiseAssist.targetStar.name);
					GUI.color = Color.white;
				}
				//GUILayout.FlexibleSpace();
				//GUILayout.Button("S");
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Target Planet: ".Translate());
				if (CruiseAssist.targetPlanet != null)
				{
					if (CruiseAssist.state == CruiseAssist.State.TO_PLANET)
					{
						GUI.color = Color.cyan;
					}
					GUILayout.Label(CruiseAssist.targetPlanet.name);
					GUI.color = Color.white;
				}
				//GUILayout.FlexibleSpace();
				//GUILayout.Button("S");
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			if (CruiseAssist.state == CruiseAssist.State.INACTIVE)
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

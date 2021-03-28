using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Tanukinomori.Patch;
using Tanukinomori.UI;
using UnityEngine;

namespace Tanukinomori
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class CruiseAssist : BaseUnityPlugin
	{
		public const string ModGuid = "tanu.CruiseAssist";
		public const string ModName = "CruiseAssist";
		public const string ModVersion = "0.0.20";

		public static bool Enable = true;
		public static bool SelectFocusFlag = false;
		public static bool HideDuplicateHistoryFlag = true;
		public static StarData ReticuleTargetStar = null;
		public static PlanetData ReticuleTargetPlanet = null;
		public static StarData SelectTargetStar = null;
		public static PlanetData SelectTargetPlanet = null;
		public static int SelectTargetAstroId = 0;
		public static StarData TargetStar = null;
		public static PlanetData TargetPlanet = null;
		public static CruiseAssistState State = CruiseAssistState.INACTIVE;

		public static List<int> History = new List<int>();
		public static List<int> Bookmark = new List<int>();

		public static Func<StarData, string> GetStarName = star => star.displayName;
		public static Func<PlanetData, string> GetPlanetName = planet => planet.displayName;

		public void Awake()
		{
			LogManager.Logger = base.Logger;
			new CruiseAssistConfigManager(base.Config);
			ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);
			var harmony = new Harmony($"{ModGuid}.Patch");
			harmony.PatchAll(typeof(Patch_GameMain));
			harmony.PatchAll(typeof(Patch_UISailPanel));
			harmony.PatchAll(typeof(Patch_UIStarmap));
			harmony.PatchAll(typeof(Patch_PlayerMoveSail));
		}

		public void OnGUI()
		{
			if (DSPGame.IsMenuDemo || GameMain.mainPlayer == null)
			{
				return;
			}
			var uiGame = UIRoot.instance.uiGame;
			if (!uiGame.guideComplete || uiGame.techTree.active || uiGame.escMenu.active || uiGame.dysonmap.active || uiGame.hideAllUI0 || uiGame.hideAllUI1)
			{
				return;
			}
			if (GameMain.mainPlayer.sailing || uiGame.starmap.active)
			{
				Check();

				CruiseAssistMainUI.wIdx = uiGame.starmap.active ? 1 : 0;

				var scale = CruiseAssistMainUI.Scale / 100.0f;

				GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Vector2.zero);

				CruiseAssistMainUI.OnGUI();
				if (CruiseAssistStarListUI.Show[CruiseAssistMainUI.wIdx])
				{
					CruiseAssistStarListUI.OnGUI();
				}
				if (CruiseAssistConfigUI.Show[CruiseAssistMainUI.wIdx])
				{
					CruiseAssistConfigUI.OnGUI();
				}
				if (CruiseAssistDebugUI.Show)
				{
					CruiseAssistDebugUI.OnGUI();
				}
			}
		}

		private void Check()
		{
			var astroId = GameMain.mainPlayer.navigation.indicatorAstroId;

			if (CruiseAssist.SelectTargetAstroId != astroId)
			{
				CruiseAssist.SelectTargetAstroId = astroId;
				if (astroId % 100 != 0)
				{
					CruiseAssist.SelectTargetPlanet = GameMain.galaxy.PlanetById(astroId);
					CruiseAssist.SelectTargetStar = CruiseAssist.SelectTargetPlanet.star;
				}
				else
				{
					CruiseAssist.SelectTargetPlanet = null;
					CruiseAssist.SelectTargetStar = GameMain.galaxy.StarById(astroId / 100);
				}
			}

			if (GameMain.localPlanet != null)
			{
				if (CruiseAssist.History.Count == 0 || CruiseAssist.History.Last() != GameMain.localPlanet.id)
				{
					if (CruiseAssist.History.Count >= 128)
					{
						CruiseAssist.History.RemoveAt(0);
					}
					CruiseAssist.History.Add(GameMain.localPlanet.id);
					ConfigManager.CheckConfig(ConfigManager.Step.STATE);
				}
			}
		}
	}
}

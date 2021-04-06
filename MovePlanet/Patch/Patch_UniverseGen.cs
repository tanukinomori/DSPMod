using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(UniverseGen))]
	public class Patch_UniverseGen
	{
		[HarmonyPatch(nameof(UniverseGen.CreateGalaxy)), HarmonyPrefix]
		public static void CreateGalaxy_Prefix()
		{
			MovePlanet.NewOldIdMap.Clear();
			MovePlanet.OldNewIdMap.Clear();
			MovePlanet.SessionEnable = false;

			if (MovePlanetMainUI.NextCheckGameTick != long.MaxValue || MovePlanetStarListUI.NextCheckGameTick != long.MaxValue || MovePlanetConfigUI.NextCheckGameTick != long.MaxValue)
			{
				ConfigManager.CheckConfig(ConfigManager.Step.STATE);

				MovePlanetMainUI.NextCheckGameTick = long.MaxValue;
				MovePlanetStarListUI.NextCheckGameTick = long.MaxValue;
				MovePlanetConfigUI.NextCheckGameTick = long.MaxValue;
			}
		}

		[HarmonyPatch(nameof(UniverseGen.CreateGalaxy)), HarmonyPostfix]
		public static void CreateGalaxy_Postfix(GalaxyData __result)
		{
			if (!DSPGame.IsMenuDemo)
			{
				MovePlanet.Seed = __result.seed;
			}

			ConfigManager.CheckConfig(ConfigManager.Step.UNIVERSE_GEN_CREATE_GALAXY);

			if (DSPGame.IsMenuDemo || !MovePlanet.ConfigEnable || MovePlanet.PlanetStarMapping.Count == 0)
			{
				return;
			}

			MovePlanet.PlanetStarMapping.OrderByDescending(tuple => tuple.v1).Do(tuple =>
			{
				MovePlanet.MovePlanetToStar(__result, tuple.v1, tuple.v2);
			});

			MovePlanet.SessionEnable = true;
		}
	}
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

// https://en.wikipedia.org/wiki/List_of_CIL_instructions

namespace Tanukinomori
{
	[HarmonyPatch(typeof(UISailPanel), "_OnUpdate")]
	public class Patch_UISailPanel_OnUpdate
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

			CodeMatcher matcher = new CodeMatcher(instructions);

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0)); // 0

			//LogManager.Logger.LogInfo("matcher.Pos=" + matcher.Pos);

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Action>(
					() => {
						CruiseAssist.targetPlanet = null;
						CruiseAssist.targetStar = null;
					}));

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Bge_Un),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Ldc_I4_1),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Stloc_S)); // 156

			//LogManager.Logger.LogInfo("matcher.Pos=" + matcher.Pos); // 156 + 1 => 157

			matcher.
				Advance(1).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0)).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StarData), nameof(StarData.planets)))).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 17)).
				InsertAndAdvance(Transpilers.EmitDelegate<Action<PlanetData[], int>>(
					(planets, planetIndex) => {
						CruiseAssist.targetPlanet = planets[planetIndex];
					}));

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Bge_Un),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Ldc_I4_1),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Stloc_S)); // 252

			//LogManager.Logger.LogInfo("matcher.Pos=" + matcher.Pos); // 252 + 1 + 4 => 257

			matcher.
				Advance(1).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 21)).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GalaxyData), nameof(GalaxyData.stars)))).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 22)).
				InsertAndAdvance(Transpilers.EmitDelegate<Action<StarData[], int>>(
					(stars, starIndex) => {
						CruiseAssist.targetStar = stars[starIndex];
					}));

			return matcher.InstructionEnumeration();
		}
	}
}

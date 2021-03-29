using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(UISailPanel))]
	public class Patch_UISailPanel
	{
		[HarmonyPatch("_OnUpdate"), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> OnUpdate_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0)); // 0

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Action>(
					() =>
					{
						CruiseAssist.ReticuleTargetPlanet = null;
						CruiseAssist.ReticuleTargetStar = null;
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

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos); // 156 + 1 => 157

			matcher.
				Advance(1).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0)).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(StarData), nameof(StarData.planets)))).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 17)).
				InsertAndAdvance(Transpilers.EmitDelegate<Action<PlanetData[], int>>(
					(planets, planetIndex) =>
					{
						CruiseAssist.ReticuleTargetPlanet = planets[planetIndex];
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

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos); // 252 + 1 + 4 => 257

			matcher.
				Advance(1).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 21)).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GalaxyData), nameof(GalaxyData.stars)))).
				InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, 22)).
				InsertAndAdvance(Transpilers.EmitDelegate<Action<StarData[], int>>(
					(stars, starIndex) =>
					{
						CruiseAssist.ReticuleTargetStar = stars[starIndex];
					}));

			return matcher.InstructionEnumeration();
		}
#if false
		private static string CLASS_NAME = "UISailPanel";

		[HarmonyPatch("_OnCreate"), HarmonyPrefix]
		public static void OnCreate_Prefix() =>
			LogManager.LogInfo($"enter {CLASS_NAME}._OnCreate");

		[HarmonyPatch("_OnDestroy"), HarmonyPrefix]
		public static void OnDestroy_Prefix() =>
			LogManager.LogInfo($"enter {CLASS_NAME}._OnDestroy");

		[HarmonyPatch("_OnInit"), HarmonyPrefix]
		public static void OnInit_Prefix() =>
			LogManager.LogInfo($"enter {CLASS_NAME}._OnInit");

		[HarmonyPatch("_OnFree"), HarmonyPrefix]
		public static void OnFree_Prefix() =>
			LogManager.LogInfo($"enter {CLASS_NAME}._OnFree");

		[HarmonyPatch("_OnUpdate"), HarmonyPrefix]
		public static void OnUpdate_Prefix() =>
			LogManager.LogInfo($"enter {CLASS_NAME}._OnUpdate");
#endif
		[HarmonyPatch("_OnOpen"), HarmonyPrefix]
		public static void OnOpen_Prefix()
		{
			if (CruiseAssist.AutoDisableLockCursorFlag)
			{
				UIRoot.instance.uiGame.disableLockCursor = true;
			}
		}

		[HarmonyPatch("_OnClose"), HarmonyPrefix]
		public static void OnClose_Prefix()
		{
			ConfigManager.CheckConfig(ConfigManager.Step.STATE);
		}
	}
}

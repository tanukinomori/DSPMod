using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Tanukinomori
{
	public class Patch_PlanetAlgorithm
	{
		[HarmonyPatch(typeof(PlanetAlgorithm7), nameof(PlanetAlgorithm7.GenerateVeins)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> PlanetAlgorithm7_GenerateVeins_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			int[] posArray = { 134, 139, 429, 549, 613 };

			for (int idx = 0; idx < posArray.Length; ++idx)
			{
				var pos = posArray[idx];

				matcher.
					MatchForward(true,
						new CodeMatch(OpCodes.Ldarg_0),
						new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "planet"),
						new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(PlanetData.star)));

				//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

				if (matcher.Pos != pos + ins)
				{
					LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
					MovePlanet.ErrorFlag = true;
					return instructions;
				}

				matcher.
					SetAndAdvance(OpCodes.Call, Transpilers.EmitDelegate<Func<PlanetData, StarData>>(MovePlanet.GetOriginalStar).operand);

				ins += 0;
			}

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(PlanetAlgorithm), nameof(PlanetAlgorithm.GenerateVeins)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> PlanetAlgorithm_GenerateVeins_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			int[] posArray = { 134, 139, 429, 567 };

			for (int idx = 0; idx < posArray.Length; ++idx)
			{
				var pos = posArray[idx];

				matcher.
					MatchForward(true,
						new CodeMatch(OpCodes.Ldarg_0),
						new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "planet"),
						new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(PlanetData.star)));

				//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

				if (matcher.Pos != pos + ins)
				{
					LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
					MovePlanet.ErrorFlag = true;
					return instructions;
				}

				matcher.
					SetAndAdvance(OpCodes.Call, Transpilers.EmitDelegate<Func<PlanetData, StarData>>(MovePlanet.GetOriginalStar).operand);

				ins += 0;
			};

			return matcher.InstructionEnumeration();
		}
	}
}

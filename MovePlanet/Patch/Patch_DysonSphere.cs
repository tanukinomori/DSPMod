using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace tanu.MovePlanet
{
	[HarmonyPatch(typeof(DysonSphere))]
	public class Patch_DysonSphere
	{
		[HarmonyPatch(nameof(DysonSphere.Init)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> Init_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(DysonSphere.starData)), // 255:183
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(StarData.planets))); // 255:184

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 184 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				MovePlanet.ErrorFlag = true;
				return instructions;
			}

			matcher.
				SetAndAdvance(OpCodes.Call, Transpilers.EmitDelegate<Func<StarData, PlanetData>>(
					star =>
					{
						return GameMain.galaxy.PlanetById(MovePlanet.GetNewId(star.id * 100 + 1));
					}).operand).
				RemoveInstructions(2);

			ins += -2;

			return matcher.InstructionEnumeration();
		}
	}
}

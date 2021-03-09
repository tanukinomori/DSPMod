using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RecycleAccumulator
{
	[HarmonyPatch(typeof(Mecha), "GenerateEnergy")]
	class Patch_Mecha_GenerateEnergy
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			CodeMatcher matcher = new CodeMatcher(instructions);
			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Mul),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Ldloc_3),
					new CodeMatch(OpCodes.Ble_Un),
					new CodeMatch(OpCodes.Ldloc_3),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Br),
					new CodeMatch(OpCodes.Ldc_I4_0)); // 66
			//RecycleAccumulator.Logger.LogInfo("matcher.Pos=" + matcher.Pos);
			var bakOpcode = matcher.Opcode;
			var bakOperand = matcher.Operand;
			matcher.
				SetAndAdvance(OpCodes.Ldarg_0, null).
				InsertAndAdvance(Transpilers.EmitDelegate<Action<Mecha>>(
					mecha => {
						if (mecha.reactorItemId == 0) {
							return;
						}
						if (mecha.reactorItemId != 2207) {
							return;
						}
						int v;
						if ((v = mecha.player.package.AddItemStacked(2206, 1)) != 0) {
							UIItemup.Up(2206, v);
						}
					})).
				InsertAndAdvance(new CodeInstruction(bakOpcode, bakOperand));
			return matcher.InstructionEnumeration();
		}
	}
}

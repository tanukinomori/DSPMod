using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace TankManualInOutAmountMultiply
{
	[HarmonyPatch(typeof(UITankWindow), "_OnUpdate")]
	static class Patch_UITankWindow_OnUpdate
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
			CodeMatcher matcher = new CodeMatcher(instructions).
				MatchForward(true,
					new CodeMatch(OpCodes.Ldloca_S),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "fluidCount"),
					new CodeMatch(OpCodes.Brtrue),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "player"),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_inhandItemCount"),
					new CodeMatch(OpCodes.Ldc_I4_1)).
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(
					inhandItemCount => inhandItemCount < TankManualInOutAmountMultiply.TankManualInOutMaxAmount ? inhandItemCount : TankManualInOutAmountMultiply.TankManualInOutMaxAmount)).
				RemoveInstructions(5).
				MatchForward(true,
					new CodeMatch(OpCodes.Ldc_I4_0),
					new CodeMatch(OpCodes.Ble),
					new CodeMatch(OpCodes.Ldloca_S),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "fluidCount"),
					new CodeMatch(OpCodes.Ldc_I4_0),
					new CodeMatch(OpCodes.Ble),
					new CodeMatch(OpCodes.Ldloca_S),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "fluidCount"),
					new CodeMatch(OpCodes.Ldc_I4_1)).
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(
					inhandItemCount => inhandItemCount < TankManualInOutAmountMultiply.TankManualInOutMaxAmount ? inhandItemCount : TankManualInOutAmountMultiply.TankManualInOutMaxAmount)).
				RemoveInstructions(5);

			return matcher.InstructionEnumeration();
		}

		static int GetTankManualInOutAmount(int inhandItemCount) {
			return inhandItemCount < TankManualInOutAmountMultiply.TankManualInOutMaxAmount ? inhandItemCount : TankManualInOutAmountMultiply.TankManualInOutMaxAmount;
		}
	}
}

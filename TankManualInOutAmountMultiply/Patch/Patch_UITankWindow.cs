using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace tanu.TankManualInOutAmountMultiply
{
	[HarmonyPatch(typeof(UITankWindow))]
	public class Patch_UITankWindow
	{
		[HarmonyPatch("_OnUpdate"), HarmonyTranspiler]
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldloc_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "fluidCount"),
					new CodeMatch(OpCodes.Brtrue),
					new CodeMatch(OpCodes.Ldarg_0), // 109:57
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "player"), // 109:58
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_inhandItemCount"), // 109:59
					new CodeMatch(OpCodes.Ldc_I4_1)); // 109:60

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 60 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				TankManualInOutAmountMultiply.ErrorFlag = true;
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(
					inhandItemCount => inhandItemCount < TankManualInOutAmountMultiply.TankManualInOutMaxAmount ? inhandItemCount : TankManualInOutAmountMultiply.TankManualInOutMaxAmount)).
				RemoveInstructions(5);

			ins += 1 - 5;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldc_I4_0),
					new CodeMatch(OpCodes.Ble),
					new CodeMatch(OpCodes.Ldloc_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "fluidCount"),
					new CodeMatch(OpCodes.Ldc_I4_0),
					new CodeMatch(OpCodes.Ble),
					new CodeMatch(OpCodes.Ldloc_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == "fluidCount"), // 134:146
					new CodeMatch(OpCodes.Ldc_I4_1)); // 134:147

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 147 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				TankManualInOutAmountMultiply.ErrorFlag = true;
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(
					inhandItemCount => inhandItemCount < TankManualInOutAmountMultiply.TankManualInOutMaxAmount ? inhandItemCount : TankManualInOutAmountMultiply.TankManualInOutMaxAmount)).
				RemoveInstructions(5);

			ins += 1 - 5;

			return matcher.InstructionEnumeration();
		}
	}
}

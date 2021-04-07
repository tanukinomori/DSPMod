using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Tanukinomori
{
	[HarmonyPatch(typeof(StationComponent))]
	public class Patch_StationComponent
	{
		delegate void StationShipAction(StationComponent station, ref ShipData ship);
		delegate int DirectionStationShipFunc(int direction, StationComponent station, ref ShipData ship);

		[HarmonyPatch(nameof(StationComponent.InternalTickRemote)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> InternalTickRemote_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();

			// 1999
			if (codes[2373].opcode != OpCodes.Ldc_I4_S)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				MovePlanet.ErrorFlag = true;
				return instructions;
			}

			codes[2373].operand = 99;

			// 2017
			if (codes[2473].opcode != OpCodes.Ldc_I4_S)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				MovePlanet.ErrorFlag = true;
				return instructions;
			}

			codes[2473].operand = 99;

			return codes;
		}
	}
}

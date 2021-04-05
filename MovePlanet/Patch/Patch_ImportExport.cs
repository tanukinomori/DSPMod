using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Tanukinomori
{
	class Patch_ImportExport
	{
		[HarmonyPatch(typeof(GameData), nameof(GameData.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> GameData_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(GameData.gameDesc)),
					new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == nameof(UniverseGen.CreateGalaxy)),
					new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == nameof(GameData.galaxy)), // 715:55
					new CodeMatch(OpCodes.Ldarg_0)); // 716:56

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 56)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Action>(() =>
				{
					ConfigManager.CheckConfig(ConfigManager.Step.UNIVERSE_GEN_CREATE_GALAXY);

					MovePlanet.NewOldIdMap.Clear();
					MovePlanet.OldNewIdMap.Clear();
					MovePlanet.SessionEnable = false;

					if (DSPGame.IsMenuDemo || !MovePlanet.ConfigEnable || MovePlanet.PlanetStarMapping.Count == 0)
					{
						return;
					}

					MovePlanet.PlanetStarMapping.OrderByDescending(tuple => tuple.v1).Do(tuple =>
					{
						MovePlanet.MovePlanetToStar(tuple.v1, tuple.v2);
					});

					MovePlanet.SessionEnable = true;
				}));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> PlanetFactory_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_3),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 3348:16
					new CodeMatch(OpCodes.Stloc_2)); // 3348:17

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 17)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetNewId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.Export)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> PlanetFactory_Export_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(i => i.opcode == OpCodes.Call && ((MethodInfo)i.operand).Name == "get_planetId"), // 3249:5
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 3249:6

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 6)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetOriginalId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(StationComponent), nameof(StationComponent.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> StationComponent_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 2579:17
					new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == nameof(StationComponent.planetId))); // 2579:18

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 18 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetNewId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(StationComponent), nameof(StationComponent.Export)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> StationComponent_Export_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(StationComponent.planetId)), // 2465:17
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 2465:18

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 18 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetOriginalId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(ShipData), nameof(ShipData.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> ShipData_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 54:9
					new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == nameof(ShipData.planetA))); // 54:10

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 10 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetNewId));

			ins += 1;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 55:13
					new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == nameof(ShipData.planetB))); // 55:14

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 14 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetNewId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(ShipData), nameof(ShipData.Export)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> ShipData_Export_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(ShipData.planetA)), // 14:9
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 14:10

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 10 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetOriginalId));

			ins += 1;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(ShipData.planetB)), // 15:13
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 15:14

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 14 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetOriginalId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(SiloComponent), nameof(SiloComponent.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> SiloComponent_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 41:13
					new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == nameof(SiloComponent.planetId))); // 41:14

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 14 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetNewId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(SiloComponent), nameof(SiloComponent.Export)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> SiloComponent_Export_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(SiloComponent.planetId)), // 15:13
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 15:14

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 14 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetOriginalId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(EjectorComponent), nameof(EjectorComponent.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> EjectorComponent_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 46:13
					new CodeMatch(i => i.opcode == OpCodes.Stfld && ((FieldInfo)i.operand).Name == nameof(EjectorComponent.planetId))); // 46:14

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 14 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetNewId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(EjectorComponent), nameof(EjectorComponent.Export)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> EjectorComponent_Export_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldarg_0),
					new CodeMatch(i => i.opcode == OpCodes.Ldfld && ((FieldInfo)i.operand).Name == nameof(EjectorComponent.planetId)), // 15:13
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 15:14

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 14 + ins)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetOriginalId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(ProductionStatistics), nameof(ProductionStatistics.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> ProductionStatistics_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 103:42
					new CodeMatch(OpCodes.Stelem_I4)); // 103:43

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 43)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetNewId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(ProductionStatistics), nameof(ProductionStatistics.Export)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> ProductionStatistics_Export_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldelem_I4), // 80:39
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 80:40

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 40)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(MovePlanet.GetOriginalId));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.Import)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> GameHistoryData_Import_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Ldloc_S),
					new CodeMatch(OpCodes.Blt),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Ldc_I4_0),
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Br),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryReader.ReadInt32)), // 1131:55
					new CodeMatch(OpCodes.Stloc_S)); // 1131:56

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 56)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(key =>
				{
					if (1020000 < key && key < 1020000 + 24000)
					{
						return MovePlanet.GetNewId(key - 1020000) + 1020000;
					}
					else if (1520000 < key && key < 1520000 + 24000)
					{
						return MovePlanet.GetNewId(key - 1520000) + 1520000;
					}
					return key;
				}));

			ins += 1;

			return matcher.InstructionEnumeration();
		}

		[HarmonyPatch(typeof(GameHistoryData), nameof(GameHistoryData.Export)), HarmonyTranspiler]
		public static IEnumerable<CodeInstruction> GameHistoryData_Export_Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			CodeMatcher matcher = new CodeMatcher(instructions);

			int ins = 0;

			matcher.
				MatchForward(true,
					new CodeMatch(OpCodes.Stloc_S),
					new CodeMatch(OpCodes.Ldarg_1),
					new CodeMatch(OpCodes.Ldloc_S), // 1067:65
					new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == nameof(BinaryWriter.Write))); // 1067:66

			//LogManager.LogInfo("matcher.Pos=" + matcher.Pos);

			if (matcher.Pos != 66)
			{
				LogManager.LogError(MethodBase.GetCurrentMethod(), "patch error.");
				return instructions;
			}

			matcher.
				InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(key =>
				{
					if (1020000 < key && key < 1020000 + 24000)
					{
						return MovePlanet.GetOriginalId(key - 1020000) + 1020000;
					}
					else if (1520000 < key && key < 1520000 + 24000)
					{
						return MovePlanet.GetOriginalId(key - 1520000) + 1520000;
					}
					return key;
				}));

			ins += 1;

			return matcher.InstructionEnumeration();
		}
	}
}

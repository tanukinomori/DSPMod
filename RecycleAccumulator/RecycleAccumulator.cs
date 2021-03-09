using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace RecycleAccumulator
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class RecycleAccumulator : BaseUnityPlugin
	{
		public const string ModGuid = "jp.co.tanukinomori.dsp.recycleaccumulator";
		public const string ModName = "RecycleAccumulator";
		public const string ModVersion = "0.0.1";

		new internal static ManualLogSource Logger;

		public void Awake() {
			RecycleAccumulator.Logger = base.Logger;
			var harmony = new Harmony("jp.co.tanukinomori.dsp.recycleaccumulator.patch");
			harmony.PatchAll(typeof(Patch_Mecha_GenerateEnergy));
		}
	}
}

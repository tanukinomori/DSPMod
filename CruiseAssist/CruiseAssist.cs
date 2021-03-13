using BepInEx;
using HarmonyLib;

namespace Tanukinomori
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class CruiseAssist : BaseUnityPlugin
	{
		public const string ModGuid = "jp.co.tanukinomori.dsp.cruiseassist";
		public const string ModName = "CruiseAssist";
		public const string ModVersion = "0.0.3";

		public static PlanetData targetPlanet;
		public static StarData targetStar;

		public void Awake() {
			LogManager.Logger = base.Logger;
			//new CruiseAssistConfigManager(base.Config);
			//ConfigManager.ConfigReload(ConfigManager.Step.AWAKE);
			var harmony = new Harmony("jp.co.tanukinomori.dsp.cruiseassist.patch");
			//harmony.PatchAll(typeof(Patch_GameMain_Begin));
			harmony.PatchAll(typeof(Patch_UISailPanel_OnUpdate));
			harmony.PatchAll(typeof(Patch_PlayerMoveSail_GameTick));
		}
	}
}
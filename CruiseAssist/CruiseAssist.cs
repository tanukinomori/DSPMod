using BepInEx;
using HarmonyLib;

namespace Tanukinomori
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class CruiseAssist : BaseUnityPlugin
	{
		public const string ModGuid = "tanu.CruiseAssist";
		public const string ModName = "CruiseAssist";
		public const string ModVersion = "0.0.7";

		public static StarData ReticuleTargetStar = null;
		public static PlanetData ReticuleTargetPlanet = null;
		public static StarData SelectTargetStar = null;
		public static PlanetData SelectTargetPlanet = null;
		public static CruiseAssistState State = CruiseAssistState.INACTIVE;
		public static bool TechTreeShow = false;

		public void Awake()
		{
			LogManager.Logger = base.Logger;
			new CruiseAssistConfigManager(base.Config);
			ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);
			var harmony = new Harmony($"{ModGuid}.Patch");
			harmony.PatchAll(typeof(Patch_GameMain));
			harmony.PatchAll(typeof(Patch_UISailPanel));
			harmony.PatchAll(typeof(Patch_UITechTree));
			harmony.PatchAll(typeof(Patch_PlayerMoveSail));
		}

		public void OnGUI()
		{
			if (CruiseAssistUI.Show && !TechTreeShow)
			{
				CruiseAssistUI.OnGUI();
			}
		}
	}
}

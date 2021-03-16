using BepInEx;
using HarmonyLib;

namespace Tanukinomori
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class CruiseAssist : BaseUnityPlugin
	{
		public const string ModGuid = "tanu.CruiseAssist";
		public const string ModName = "CruiseAssist";
		public const string ModVersion = "0.0.6";

		public static StarData TargetStar = null;
		public static PlanetData TargetPlanet = null;
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

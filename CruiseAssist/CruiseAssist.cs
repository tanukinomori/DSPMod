using BepInEx;
using HarmonyLib;

namespace Tanukinomori
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class CruiseAssist : BaseUnityPlugin
	{
		public const string ModGuid = "tanu.CruiseAssist";
		public const string ModName = "CruiseAssist";
		public const string ModVersion = "0.0.4";

		public enum State { TO_STAR, TO_PLANET, INACTIVE };

		public static StarData targetStar = null;
		public static PlanetData targetPlanet = null;
		public static State state = State.INACTIVE;
		public static bool TechTreeShow = false;

		public void Awake()
		{
			LogManager.Logger = base.Logger;
			new CruiseAssistConfigManager(base.Config);
			ConfigManager.ConfigReload(ConfigManager.Step.AWAKE);
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

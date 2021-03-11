using HarmonyLib;
using System.Linq;

namespace AutoReplenishItem
{
	[HarmonyPatch(typeof(UIStorageWindow), "_OnUpdate")]
	public class Patch_UIStorageWindow_OnUpdate
	{
		[HarmonyPrefix]
		public static void Prefix(UIStorageWindow __instance) =>
			Patch_UIStorageWindow_OnOpen.Prefix(__instance);
	}
}

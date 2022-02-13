using HarmonyLib;
using System.Linq;

namespace AutoReplenishItem
{
	[HarmonyPatch(typeof(UIStorageWindow), "_OnOpen")]
	public class Patch_UIStorageWindow_OnOpen
	{
		[HarmonyPrefix]
		public static void Prefix(UIStorageWindow __instance) {
			if (GameMain.localPlanet == null || GameMain.localPlanet.factory == null) {
				return;
			}
			var factoryStorage = GameMain.localPlanet.factory.factoryStorage;
			var storageComponent = factoryStorage.storagePool[__instance.storageId];
			if (storageComponent == null) {
				return;
			}

			foreach (var entity in AutoReplenishItem.AutoReplenishItemMap) {
				var itemId = entity.Key;
				var eplenishItemCount = entity.Value;
				var playerItemCount = GameMain.mainPlayer.package.GetItemCount(itemId);
				var storageItemCount = storageComponent.GetItemCount(itemId);
				if (playerItemCount < eplenishItemCount) {
					var requireItemCount = eplenishItemCount - playerItemCount;
					var takeItemCount = storageComponent.TakeItem(itemId, requireItemCount, out int inc);
					GameMain.mainPlayer.package.AddItem(itemId, takeItemCount, inc, out int remainInc);
				}
			}
		}
	}
}

using HarmonyLib;
using UnityEngine;

namespace FoundationMask
{
	[HarmonyPatch(typeof(PlanetGrid), "ReformSnapTo")]
	static class Patch_PlanetGrid_ReformSnapTo
	{
		[HarmonyPrefix]
		public static bool Prefix(PlanetGrid __instance, ref int __result, ref Vector3 pos, ref int reformSize, ref int reformType, ref int reformColor, ref Vector3[] reformPoints, ref int[] reformIndices, ref PlatformSystem platform, out Vector3 reformCenter) {
			pos.Normalize();
			float num = Mathf.Asin(pos.y);
			float num2 = Mathf.Atan2(pos.x, -pos.z);
			float num3 = num / 6.2831855f * (float)__instance.segment;
			int latitudeIndex = Mathf.FloorToInt(Mathf.Abs(num3));
			int num4 = PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment);
			float num5 = (float)num4;
			float num6 = num2 / 6.2831855f * num5;
			float num7 = Mathf.Round(num3 * 10f);
			float num8 = Mathf.Round(num6 * 10f);
			float num9 = Mathf.Abs(num7);
			float num10 = Mathf.Abs(num8);
			int num11 = reformSize % 2;
			if (num9 % 2f != (float)num11) {
				num3 = Mathf.Abs(num3);
				num9 = (float)Mathf.FloorToInt(num3 * 10f);
				if (num9 % 2f != (float)num11) {
					num9 += 1f;
				}
			}
			num9 = ((num7 < 0f) ? (-num9) : num9);
			if (num10 % 2f != (float)num11) {
				num6 = Mathf.Abs(num6);
				num10 = (float)Mathf.FloorToInt(num6 * 10f);
				if (num10 % 2f != (float)num11) {
					num10 += 1f;
				}
			}
			num10 = ((num8 < 0f) ? (-num10) : num10);
			num = num9 / 10f / (float)__instance.segment * 6.2831855f;
			num2 = num10 / 10f / num5 * 6.2831855f;
			float y = Mathf.Sin(num);
			float num12 = Mathf.Cos(num);
			float num13 = Mathf.Sin(num2);
			float num14 = Mathf.Cos(num2);
			reformCenter = new Vector3(num12 * num13, y, num12 * -num14);
			int num15 = 1 - reformSize;
			int num16 = 1 - reformSize;
			int num17 = 0;
			int num18 = 0;
			float num19 = (float)(platform.latitudeCount / 10);
			for (int i = 0; i < reformSize * reformSize; i++) {
				num18++;
				num3 = (num9 + (float)num15) / 10f;
				num6 = (num10 + (float)num16) / 10f;
				num16 += 2;
				if (num18 % reformSize == 0) {
					num16 = 1 - reformSize;
					num15 += 2;
				}
				if (num3 >= num19 || num3 <= -num19) {
					reformIndices[i] = -1;
				} else {
					latitudeIndex = Mathf.FloorToInt(Mathf.Abs(num3));
					if (num4 != PlanetGrid.DetermineLongitudeSegmentCount(latitudeIndex, __instance.segment)) {
						reformIndices[i] = -1;
					} else {
						int reformX;
						int reformY;
						int reformIndexForSegment = platform_GetReformIndexForSegment(platform, num3, num6, out reformX, out reformY);
						if (FoundationMask.ConfigMask[reformY % FoundationMask.ConfigMaskSizeY][reformX % FoundationMask.ConfigMaskSizeX] != '1') {
							continue;
						}
						reformIndices[i] = reformIndexForSegment;
						int reformType2 = platform.GetReformType(reformIndexForSegment);
						int reformColor2 = platform.GetReformColor(reformIndexForSegment);
						if (!platform.IsTerrainReformed(reformType2) && (reformType2 != reformType || reformColor2 != reformColor)) {
							num = num3 / (float)__instance.segment * 6.2831855f;
							num2 = num6 / num5 * 6.2831855f;
							y = Mathf.Sin(num);
							num12 = Mathf.Cos(num);
							num13 = Mathf.Sin(num2);
							num14 = Mathf.Cos(num2);
							reformPoints[num17] = new Vector3(num12 * num13, y, num12 * -num14);
							num17++;
						}
					}
				}
			}
			__result = num17;
			return false;
		}

		private static int platform_GetReformIndexForSegment(PlatformSystem _platform, float _latitudeSeg, float _longitudeSeg, out int x, out int y) {
			int num = (_latitudeSeg <= 0f) ? Mathf.FloorToInt(_latitudeSeg * 5f) : Mathf.CeilToInt(_latitudeSeg * 5f);
			int num2 = (_longitudeSeg <= 0f) ? Mathf.FloorToInt(_longitudeSeg * 5f) : Mathf.CeilToInt(_longitudeSeg * 5f);
			int num3 = _platform.latitudeCount / 2;
			y = (num <= 0) ? (num3 - num - 1) : (num - 1);
			int latitudeIndex = Mathf.FloorToInt(Mathf.Abs(_latitudeSeg));
			int num4 = PlatformSystem.DetermineLongitudeSegmentCount(latitudeIndex, _platform.segment);
			if (num2 > num4 * 5 / 2) {
				num2 = num2 - num4 * 5 - 1;
			}
			if (num2 < -num4 * 5 / 2) {
				num2 = num4 * 5 + num2 + 1;
			}
			x = (num2 <= 0) ? (num4 * 5 / 2 - num2 - 1) : (num2 - 1);
			return _platform.GetReformIndex(x, y);
		}
	}
}

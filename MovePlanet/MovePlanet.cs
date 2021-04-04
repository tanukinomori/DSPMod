using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tanukinomori
{
	[BepInPlugin(ModGuid, ModName, ModVersion)]
	public class MovePlanet : BaseUnityPlugin
	{
		public const string ModGuid = "tanu.MovePlanet";
		public const string ModName = "MovePlanet";
		public const string ModVersion = "0.0.1";

		public static bool ConfigEnable = true;
		public static bool SessionEnable = true;

		public static List<Tuple<int, int>> PlanetStarMapping = new List<Tuple<int, int>>();

		public static Dictionary<int, int> NewOldIdMap = new Dictionary<int, int>();
		public static Dictionary<int, int> OldNewIdMap = new Dictionary<int, int>();

		public void Awake()
		{
			LogManager.Logger = base.Logger;
			new MovePlanetConfigManager(base.Config);
			ConfigManager.CheckConfig(ConfigManager.Step.AWAKE);
			var harmony = new Harmony($"{ModGuid}.Patch");
			harmony.PatchAll(typeof(Patch_GameMain));
			harmony.PatchAll(typeof(Patch_ImportExport));
			harmony.PatchAll(typeof(Patch_StationComponent));
			harmony.PatchAll(typeof(Patch_DysonSphere));
		}

		public static void MovePlanetToStar(int targetPlanetId, int toStarId)
		{
			var planet = GameMain.galaxy.PlanetById(targetPlanetId);
			var toStar = GameMain.galaxy.StarById(toStarId);
			var fromStar = planet.star;

			if (planet.orbitAroundPlanet != null)
			{
				// 衛星は単独で移動しない
				return;
			}
			if (NewOldIdMap.ContainsKey(planet.id))
			{
				// 移動済みは移動しない
				return;
			}

			var fromPlanetList = new List<PlanetData>();
			var toPlanetList = toStar.planets.ToList();

			int lastNumber;

			lastNumber = 0;
			for (int fromIndex = 0; fromIndex < fromStar.planetCount; ++fromIndex)
			{
				var p = fromStar.planets[fromIndex];
				if (p.id == targetPlanetId)
				{
					p.number = toPlanetList.Where(p2 => p2.orbitAroundPlanet == null).Max(p2 => p2.number) + 1;
				}
				else if (p.orbitAroundPlanet != null && p.orbitAroundPlanet.id == planet.id)
				{
				}
				else
				{
					fromPlanetList.Add(p);
					continue;
				}
				toPlanetList.Add(p);
				p.star = toStar;
				p.index = toPlanetList.Count - 1;
				var oldId = p.id;
				var newId = toStar.id * 100 + p.index + 1;
				p.id = newId;
				AddNewOldId(newId, oldId);
				GameMain.galaxy.astroPoses[newId].uRadius = p.realRadius;
				GameMain.galaxy.astroPoses[oldId].uRadius = 0f;
			}

			toStar.planets = toPlanetList.ToArray();
			toStar.planetCount = toStar.planets.Length;

			fromStar.planets = fromPlanetList.ToArray();
			fromStar.planetCount = fromStar.planets.Length;

			lastNumber = 0;
			for (int i = 0; i < fromStar.planetCount; ++i)
			{
				var p = fromStar.planets[i];
				p.index = i;
				var oldId = p.id;
				var newId = fromStar.id * 100 + p.index + 1;
				if (newId != oldId)
				{
					p.id = newId;
					AddNewOldId(newId, oldId);
					GameMain.galaxy.astroPoses[newId].uRadius = p.realRadius;
					GameMain.galaxy.astroPoses[oldId].uRadius = 0f;
				}
				if (p.orbitAroundPlanet == null)
				{
					p.number = lastNumber + 1;
					lastNumber = p.number;
				}
			}

			lastNumber = 0;
			toPlanetList.Do(
				p =>
				{
					if (p.orbitAroundPlanet == null)
					{
						p.number = lastNumber + 1;
						lastNumber = p.number;
					}
				});

			//LogManager.LogInfo($"id={planet.id}, name={planet.displayName}");
		}

		public static void AddNewOldId(int newId, int oldId)
		{
			if (newId == oldId)
			{
				return;
			}

			if (NewOldIdMap.TryGetValue(oldId, out var id))
			{
				NewOldIdMap.Remove(oldId);
				OldNewIdMap.Remove(id);
				//LogManager.LogInfo($"remove {id}=>{oldId}");
				NewOldIdMap.Add(newId, id);
				OldNewIdMap.Add(id, newId);
				//LogManager.LogInfo($"add {id}=>{newId}");
			}
			else
			{
				NewOldIdMap.Add(newId, oldId);
				OldNewIdMap.Add(oldId, newId);
				//LogManager.LogInfo($"add {oldId}=>{newId}");
			}
		}

		public static int GetOriginalId(int id)
		{
			if (!MovePlanet.SessionEnable)
			{
				return id;
			}
			if (NewOldIdMap.TryGetValue(id, out var oldId))
			{
				//LogManager.LogInfo($"get {oldId}<={id}");
				return oldId;
			}
			return id;
		}

		public static int GetNewId(int id)
		{
			if (!MovePlanet.SessionEnable)
			{
				return id;
			}
			if (OldNewIdMap.TryGetValue(id, out var newId))
			{
				//LogManager.LogInfo($"get {id}=>{newId}");
				return newId;
			}
			return id;
		}

		public void OnGUI()
		{
			if (DSPGame.IsMenuDemo || GameMain.mainPlayer == null)
			{
				return;
			}
			var uiGame = UIRoot.instance.uiGame;
			if (!uiGame.guideComplete || uiGame.techTree.active || uiGame.escMenu.active || uiGame.dysonmap.active || uiGame.hideAllUI0 || uiGame.hideAllUI1)
			{
				return;
			}
			if (uiGame.starmap.active)
			{
				var scale = MovePlanetMainUI.Scale / 100.0f;

				GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), Vector2.zero);

				MovePlanetMainUI.OnGUI();
				if (MovePlanetStarListUI.Show)
				{
					MovePlanetStarListUI.OnGUI();
				}
				if (MovePlanetConfigUI.Show)
				{
					MovePlanetConfigUI.OnGUI();
				}
			}
		}
	}
}

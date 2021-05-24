using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace tanu.MovePlanet
{
	public class MovePlanetConfigManager : ConfigManager
	{
		public MovePlanetConfigManager(ConfigFile Config) : base(Config)
		{
		}
		protected override void CheckConfigImplements(Step step)
		{
			bool saveFlag = false;

			if (step == Step.AWAKE)
			{
				var modVersion = Bind<string>("Base", "ModVersion", MovePlanet.ModVersion, "Don't change.");
				modVersion.Value = MovePlanet.ModVersion;

				Migration("State", "MainWindow0Left", 100, "State", "MainWindowLeft");
				Migration("State", "MainWindow0Top", 100, "State", "MainWindowTop");
				Migration("State", "ConfigWindow0Left", 100, "State", "ConfigWindowLeft");
				Migration("State", "ConfigWindow0Top", 100, "State", "ConfigWindowTop");

				saveFlag = true;
			}
			if (step == Step.AWAKE || step == Step.UNIVERSE_GEN_CREATE_GALAXY)
			{
				MovePlanet.ConfigEnable = Bind("Setting", "Enable", true).Value;

				MovePlanet.MovePlayerFlag = Bind("Setting", "MovePlayer", true).Value;
				MovePlanet.LoadWarperFlag = Bind("Setting", "LoadWarper", true).Value;
				MovePlanet.MarkVisitedFlag = Bind("Setting", "MarkVisited", true).Value;

				MovePlanetMainUI.Scale = (float)Bind("Setting", "UIScale", 150).Value;

				for (int i = 0; i < 2; ++i)
				{
					MovePlanetMainUI.Rect[i].x = (float)Bind("State", $"MainWindow{i}Left", 100).Value;
					MovePlanetMainUI.Rect[i].y = (float)Bind("State", $"MainWindow{i}Top", 100).Value;
					MovePlanetConfigUI.Rect[i].x = (float)Bind("State", $"ConfigWindow{i}Left", 100).Value;
					MovePlanetConfigUI.Rect[i].y = (float)Bind("State", $"ConfigWindow{i}Top", 100).Value;
				}

				MovePlanetStarListUI.Rect.x = (float)Bind("State", "StarListWindowLeft", 100).Value;
				MovePlanetStarListUI.Rect.y = (float)Bind("State", "StarListWindowTop", 100).Value;

				if (!DSPGame.IsMenuDemo && GameMain.galaxy != null)
				{
					MovePlanet.PlanetStarMapping =
						ListUtils.ParseToStringList(Bind("Save", $"Mapping_{GameMain.galaxy.seed}", "").Value).
						Select(str =>
						{
							var array = str.Split('-');
							return new Tuple<int, int>(int.Parse(array[0]), int.Parse(array[1]));
						}).ToList();
				}
				else
				{
					MovePlanet.PlanetStarMapping = new List<Tuple<int, int>>();
				}
			}
			else if (step == Step.STATE)
			{
				LogManager.LogInfo("check state.");

				saveFlag |= UpdateEntry("Setting", "Enable", MovePlanet.ConfigEnable);

				saveFlag |= UpdateEntry("Setting", "MovePlayer", MovePlanet.MovePlayerFlag);
				saveFlag |= UpdateEntry("Setting", "LoadWarper", MovePlanet.LoadWarperFlag);
				saveFlag |= UpdateEntry("Setting", "MarkVisited", MovePlanet.MarkVisitedFlag);

				saveFlag |= UpdateEntry("Setting", "UIScale", (int)MovePlanetMainUI.Scale);

				for (int i = 0; i < 2; ++i)
				{
					saveFlag |= UpdateEntry("State", $"MainWindow{i}Left", (int)MovePlanetMainUI.Rect[i].x);
					saveFlag |= UpdateEntry("State", $"MainWindow{i}Top", (int)MovePlanetMainUI.Rect[i].y);
					saveFlag |= UpdateEntry("State", $"ConfigWindow{i}Left", (int)MovePlanetConfigUI.Rect[i].x);
					saveFlag |= UpdateEntry("State", $"ConfigWindow{i}Top", (int)MovePlanetConfigUI.Rect[i].y);
				}

				saveFlag |= UpdateEntry("State", "StarListWindowLeft", (int)MovePlanetStarListUI.Rect.x);
				saveFlag |= UpdateEntry("State", "StarListWindowTop", (int)MovePlanetStarListUI.Rect.y);

				if (!DSPGame.IsMenuDemo && GameMain.galaxy != null)
				{
					if (!ContainsKey("Save", $"Mapping_{GameMain.galaxy.seed}"))
					{
						Bind("Save", $"Mapping_{GameMain.galaxy.seed}", "");
						saveFlag = true;
					}
					saveFlag |= UpdateEntry("Save", $"Mapping_{GameMain.galaxy.seed}", ListUtils.ToString(MovePlanet.PlanetStarMapping.Select(tuple => $"{tuple.v1}-{tuple.v2}").ToList()));
				}

				MovePlanetMainUI.NextCheckGameTick = long.MaxValue;
			}
			if (saveFlag)
			{
				Save(false);
			}
		}
	}
}

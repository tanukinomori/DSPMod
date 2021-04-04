using BepInEx.Configuration;
using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;

namespace Tanukinomori
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

				saveFlag = true;
			}
			if (step == Step.AWAKE || step == Step.UNIVERSE_GEN_CREATE_GALAXY)
			{
				MovePlanet.ConfigEnable = Bind("Setting", "Enable", true).Value;

				MovePlanetMainUI.Rect.x = (float)Bind("State", "MainWindowLeft", 100).Value;
				MovePlanetMainUI.Rect.y = (float)Bind("State", "MainWindowTop", 100).Value;

				MovePlanetStarListUI.Rect.x = (float)Bind("State", "StarListWindowLeft", 100).Value;
				MovePlanetStarListUI.Rect.y = (float)Bind("State", "StarListWindowTop", 100).Value;

				MovePlanetConfigUI.Rect.x = (float)Bind("State", "ConfigWindowLeft", 100).Value;
				MovePlanetConfigUI.Rect.y = (float)Bind("State", "ConfigWindowTop", 100).Value;

				if (GameMain.galaxy != null)
				{
					MovePlanet.PlanetStarMapping =
						ListUtils.ParseToStringList(Bind("Save", $"Mapping_{GameMain.galaxy.seed}", "").Value).
						Select(str =>
						{
							var array = str.Split('-');
							return new Tuple<int, int>(int.Parse(array[0]), int.Parse(array[1]));
						}).ToList();
				}
			}
			else if (step == Step.STATE)
			{
				saveFlag |= UpdateEntry("Setting", "Enable", MovePlanet.ConfigEnable);

				saveFlag |= UpdateEntry("State", "MainWindowLeft", (int)MovePlanetMainUI.Rect.x);
				saveFlag |= UpdateEntry("State", "MainWindowTop", (int)MovePlanetMainUI.Rect.y);

				saveFlag |= UpdateEntry("State", "StarListWindowLeft", (int)MovePlanetStarListUI.Rect.x);
				saveFlag |= UpdateEntry("State", "StarListWindowTop", (int)MovePlanetStarListUI.Rect.y);

				saveFlag |= UpdateEntry("State", "ConfigWindowLeft", (int)MovePlanetConfigUI.Rect.x);
				saveFlag |= UpdateEntry("State", "ConfigWindowTop", (int)MovePlanetConfigUI.Rect.y);

				saveFlag |= UpdateEntry("Save", $"Mapping_{GameMain.galaxy.seed}", ListUtils.ToString(MovePlanet.PlanetStarMapping.Select(tuple => $"{tuple.v1}-{tuple.v2}").ToList()));
			}
			if (saveFlag)
			{
				Save(false);
			}
		}
	}
}

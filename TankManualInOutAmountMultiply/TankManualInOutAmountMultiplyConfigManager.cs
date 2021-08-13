using BepInEx.Configuration;

namespace tanu.TankManualInOutAmountMultiply
{
	public class TankManualInOutAmountMultiplyConfigManager : ConfigManager
	{
		public TankManualInOutAmountMultiplyConfigManager(ConfigFile Config) : base(Config)
		{
		}

		protected override void CheckConfigImplements(Step step)
		{
			bool saveFlag = false;

			if (step == Step.AWAKE)
			{
				var modVersion = Bind<string>("Base", "ModVersion", TankManualInOutAmountMultiply.ModVersion, "Don't change.");
				modVersion.Value = TankManualInOutAmountMultiply.ModVersion;

				saveFlag = true;
			}
			if (step == Step.AWAKE || step == Step.GAME_MAIN_BEGIN)
			{
				TankManualInOutAmountMultiply.ConfigMultiValue = Bind<int>("Base", "MultiplyValue", 5).Value;
				TankManualInOutAmountMultiply.TankManualInOutMaxAmount = 2 * TankManualInOutAmountMultiply.ConfigMultiValue;
			}

			if (saveFlag)
			{
				Save(true);
			}
		}
	}
}

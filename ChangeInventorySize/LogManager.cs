using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;

namespace Tanukinomori
{
	public static class LogManager
	{
		public static ManualLogSource Logger { get; set; }
	}
}

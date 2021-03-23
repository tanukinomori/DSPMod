using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Tanukinomori.CruiseAssistMainUIViewMode;

namespace Tanukinomori
{
	[TestClass]
	public class EnumUtilsUnitTest
	{
		[TestMethod]
		public void TryParse001()
		{
			EnumUtils.TryParse<CruiseAssistMainUIViewMode>(null, out var viewMode);
			Assert.AreEqual(FULL, viewMode);
		}

		[TestMethod]
		public void TryParse002()
		{
			EnumUtils.TryParse<CruiseAssistMainUIViewMode>("", out var viewMode);
			Assert.AreEqual(FULL, viewMode);
		}

		[TestMethod]
		public void TryParse003()
		{
			EnumUtils.TryParse<CruiseAssistMainUIViewMode>("FULL", out var viewMode);
			Assert.AreEqual(FULL, viewMode);
		}

		[TestMethod]
		public void TryParse004()
		{
			EnumUtils.TryParse<CruiseAssistMainUIViewMode>("MINI", out var viewMode);
			Assert.AreEqual(MINI, viewMode);
		}

		[TestMethod]
		public void TryParse005()
		{
			EnumUtils.TryParse<CruiseAssistMainUIViewMode>("AAAA", out var viewMode);
			Assert.AreEqual(FULL, viewMode);
		}
	}
}

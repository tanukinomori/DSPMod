using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Tanukinomori.commons;

namespace Tanukinomori
{
	[TestClass]
	public class ListUtilsUnitTest
	{
		[TestMethod]
		public void ToStringTest001()
		{
			Assert.AreEqual("", ListUtils.ToString(null));
		}

		[TestMethod]
		public void ToStringTest002()
		{
			var list = new List<int>();
			Assert.AreEqual("", ListUtils.ToString(list));
		}

		[TestMethod]
		public void ToStringTest003()
		{
			var list = new List<int>();
			list.Add(123);
			Assert.AreEqual("123", ListUtils.ToString(list));
		}

		[TestMethod]
		public void ToStringTest004()
		{
			var list = new List<int>();
			list.Add(123);
			list.Add(456);
			Assert.AreEqual("123,456", ListUtils.ToString(list));
		}

		[TestMethod]
		public void ToStringTest005()
		{
			var list = new List<int>();
			list.Add(123);
			list.Add(456);
			list.Add(789);
			Assert.AreEqual("123,456,789", ListUtils.ToString(list));
		}

		[TestMethod]
		public void ParseTest001()
		{
			var list = new List<int>();
			CollectionAssert.AreEquivalent(list, ListUtils.Parse(null));
		}

		[TestMethod]
		public void ParseTest002()
		{
			var list = new List<int>();
			CollectionAssert.AreEquivalent(list, ListUtils.Parse(""));
		}

		[TestMethod]
		public void ParseTest003()
		{
			var list = new List<int>();
			list.Add(123);
			CollectionAssert.AreEquivalent(list, ListUtils.Parse("123"));
		}

		[TestMethod]
		public void ParseTest004()
		{
			var list = new List<int>();
			list.Add(123);
			list.Add(456);
			CollectionAssert.AreEquivalent(list, ListUtils.Parse("123,456"));
		}

		[TestMethod]
		public void ParseTest005()
		{
			var list = new List<int>();
			list.Add(123);
			list.Add(456);
			list.Add(789);
			CollectionAssert.AreEquivalent(list, ListUtils.Parse("123,456,789"));
		}
	}
}

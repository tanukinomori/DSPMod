using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tanukinomori.commons
{
	public class ListUtils
	{
		public static string ToString(List<int> list)
		{
			if (list == null || list.Count == 0)
			{
				return "";
			}
			return list.Select(id => id.ToString()).Aggregate((a, b) => a + "," + b);
		}

		public static List<int> Parse(string str)
		{
			if (str == null || str.Length == 0)
			{
				return new List<int>();
			}
			return str.Split(',').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
		}
	}
}

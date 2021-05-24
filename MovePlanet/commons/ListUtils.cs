using System.Collections.Generic;
using System.Linq;

namespace tanu.MovePlanet
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

		public static string ToString(List<string> list)
		{
			if (list == null || list.Count == 0)
			{
				return "";
			}
			return list.Aggregate((a, b) => a + "," + b);
		}

		public static List<int> ParseToIntList(string str)
		{
			if (str == null || str.Length == 0)
			{
				return new List<int>();
			}
			return str.Split(',').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
		}

		public static List<string> ParseToStringList(string str)
		{
			if (str == null || str.Length == 0)
			{
				return new List<string>();
			}
			return str.Split(',').ToList();
		}
	}
}

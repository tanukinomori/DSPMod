using System;

namespace Tanukinomori
{
	public class EnumUtils
	{
		public static bool TryParse<TEnum>(string value, out TEnum result)
		{
			if (value == null || !Enum.IsDefined(typeof(TEnum), value))
			{
				result = (TEnum)Enum.GetValues(typeof(TEnum)).GetValue(0);
				return false;
			}
			result = (TEnum)Enum.Parse(typeof(TEnum), value);
			return true;
		}
	}
}

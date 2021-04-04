namespace Tanukinomori
{
	public struct Tuple<T1, T2>
	{
		public T1 v1 { get; set; }
		public T2 v2 { get; set; }

		public Tuple(T1 v1, T2 v2)
		{
			this.v1 = v1;
			this.v2 = v2;
		}
	}
}

namespace AB.Common.Inflectors
{
	public static class Inflector
	{
		static Inflector()
		{
			English = new EnglishInflector();
			Spanish = new SpanishInflector();
		}

		public static IInflector English { get; private set; }
		public static IInflector Spanish { get; private set; }
	}
}
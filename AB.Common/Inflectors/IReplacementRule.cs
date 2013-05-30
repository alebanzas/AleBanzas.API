namespace AB.Common.Inflectors
{
	public interface IReplacementRule : IRuleApplier
	{
		string Replacement { get; }
		string Pattern { get; }
	}
}
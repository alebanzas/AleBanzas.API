namespace AB.Common.Inflectors
{
	/// <summary>
	/// Inflector for pluralize and singularize nouns.
	/// </summary>
	/// <remarks>
	/// Inspired from Bermi Ferrer Martinez python implementation.
	/// </remarks>
	public interface IInflector
	{
		/// <summary>
		/// Pluralizes nouns.
		/// </summary>
		/// <param name="word">Singular noun.</param>
		/// <returns>Plural noun.</returns>
		string Pluralize(string word);

		/// <summary>
		/// Singularizes nouns.
		/// </summary>
		/// <param name="word">Plural noun.</param>
		/// <returns>Singular noun.</returns>
		string Singularize(string word);

		string Ordinalize(string number);
	}
}
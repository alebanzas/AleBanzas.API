using System.IO;

namespace ABServicios.Azure.Storage
{
	/// <summary>
	/// Provides functionality for serialize/deserialize documents.
	/// </summary>
	public interface IDocumentSerializer<TDocument> where TDocument: class
	{
		/// <summary>
		/// Serializes an document to the provided stream.
		/// </summary>
		/// <param name="document">The document graph to serialize. </param>
		/// <returns>The stream where the serializer puts the serialized data.</returns>
		Stream Serialize(TDocument document);

		/// <summary>
		/// Deserializes the data on the provided stream and reconstitutes the graph of document.
		/// </summary>
		/// <param name="documentStream">The stream that contains the data to deserialize. </param>
		/// <returns>The deserialized document.</returns>
		TDocument Deserialize(Stream documentStream);
	}
}
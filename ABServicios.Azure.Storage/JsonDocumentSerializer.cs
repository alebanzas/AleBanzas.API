using System.IO;
using Newtonsoft.Json;

namespace ABServicios.Azure.Storage
{
	public class JsonDocumentSerializer<TDocument> : IDocumentSerializer<TDocument> where TDocument : class
	{
		public Stream Serialize(TDocument document)
		{
			var serialized = JsonConvert.SerializeObject(document);
			var serializationStream = new MemoryStream(serialized.Length * 4);
			var writer = new StreamWriter(serializationStream);
			writer.Write(serialized);
			writer.Flush();
			serializationStream.Seek(0L, SeekOrigin.Begin);

			return serializationStream;
		}

		public TDocument Deserialize(Stream documentStream)
		{
			documentStream.Seek(0L, SeekOrigin.Begin);
			var reader = new StreamReader(documentStream);
			string s = reader.ReadToEnd();
			return JsonConvert.DeserializeObject<TDocument>(s);
		}
	}
}
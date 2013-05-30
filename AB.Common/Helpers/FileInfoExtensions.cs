using System.IO;
using System.IO.Compression;

namespace AB.Common.Helpers
{
	public static class FileInfoExtensions
	{
		private const int FourKB = 0x1000;
		public static void GZipCompress(this FileInfo source)
		{
			using (FileStream inFile = source.OpenRead())
			{
				if ((File.GetAttributes(source.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & source.Extension != ".gz")
				{
					using (FileStream outFile = File.Create(source.FullName + ".gz"))
					{
						using (var compress = new GZipStream(outFile, CompressionMode.Compress))
						{
							inFile.CopyTo(compress);
							compress.Flush();
						}
					}
				}
			}
		}

		public static void GZipDecompress(this FileInfo source)
		{
			using (FileStream inFile = source.OpenRead())
			{
				string curFile = source.FullName;
				string origName = curFile.Remove(curFile.Length -source.Extension.Length);

				using (FileStream outFile = File.Create(origName))
				{
					using (var decompress = new GZipStream(inFile,CompressionMode.Decompress))
					{
						decompress.CopyTo(outFile);
						decompress.Flush();
					}
				}
			}
		}

		private static void CopyTo(this Stream source, Stream destination)
		{
			int readedSize;
			// lee y escrive usando un buffer de 4KB (evitamos de tomarnos toda la RAM si el file es grande)
			var buffer = new byte[FourKB];
			while ((readedSize = source.Read(buffer, 0, buffer.Length)) != 0)
			{
				destination.Write(buffer, 0, readedSize);
			}
		}
	}
}
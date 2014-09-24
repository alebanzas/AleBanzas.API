using System;
using System.Net;

namespace ABServicios.Azure.Storage
{
	public static class WebClientExtension
	{
		public static byte[] TryDownloadData(this WebClient source, string fileUrl)
		{
			try
			{
				return source.DownloadData(fileUrl);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
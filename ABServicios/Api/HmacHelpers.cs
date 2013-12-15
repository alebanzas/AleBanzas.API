using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ABServicios.Api
{
	public static class HmacHelpers
	{
		public static async Task<byte[]> ComputeMd5Hash(this HttpContent source)
		{
			using (MD5 md5 = MD5.Create())
			{
				var content = await source.ReadAsByteArrayAsync();
				byte[] hash = md5.ComputeHash(content);
				return hash;
			}
		}
	}
}
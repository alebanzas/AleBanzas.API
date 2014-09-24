using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AB.Common.Helpers
{
	public static class ImageExtensions
	{
		public static void ApplyWatermark(this Image source, Image watermark)
		{
			ApplyWatermark(source, watermark, 1F);
		}

		public static void ApplyWatermark(this Image source, Image watermark, float transparency)
		{
			//Resize Watermark
			var percentageResize = (source.Width*100/watermark.Width);
			var newHeigh = (watermark.Height*percentageResize/100);
			using (var resizedWatermark = watermark.Resize(source.Width, newHeigh))
			{
				//Create graphics object of the background image so that you can draw your logo on it
				using (Graphics g = Graphics.FromImage(source))
				{
					//Create a blank bitmap object to which we draw our transparent logo
					using (var transparent = new Bitmap(resizedWatermark.Width, resizedWatermark.Height))
					{
						using (Graphics graphics = Graphics.FromImage(transparent))
						{
							//An image is represenred as a 5X4 matrix(i.e 4 columns and 5 rows) 
							//the 3rd element of the 4th row represents the transparency
							var colorMatrix = new ColorMatrix {Matrix33 = transparency};

							//an ImageAttributes object is used to set all the alpha values.
							//This is done by initializing a color matrix and setting the alpha scaling value in the matrix.
							//The address of the color matrix is passed to the SetColorMatrix method of the ImageAttributes object, 
							//and the ImageAttributes object is passed to the DrawImage method of the Graphics object.
							var imgAttributes = new ImageAttributes();
							imgAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

							graphics.DrawImage(resizedWatermark, new Rectangle(0, 0, transparent.Width, transparent.Height), 0, 0, transparent.Width, transparent.Height, GraphicsUnit.Pixel,
							                   imgAttributes);
						}
						g.DrawImage(transparent, 0, (source.Height/2) - (resizedWatermark.Height/2), transparent.Width, transparent.Height);
						g.Flush();
					}
				}
			}
		}

		public static Image Resize(this Image source, int intWidth, int intHeight)
		{
			if (source == null)
			{
				return null;
			}
			Image img = new Bitmap(intWidth, intHeight, source.PixelFormat);
			using (Graphics graphic = Graphics.FromImage(img))
			{
				graphic.CompositingQuality = CompositingQuality.HighQuality;
				graphic.SmoothingMode = SmoothingMode.HighQuality;
				graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
				//objGraphic.InterpolationMode = InterpolationMode.NearestNeighbor;

				var oRectangle = new Rectangle(0, 0, intWidth, intHeight);
				graphic.DrawImage(source, oRectangle);
				graphic.Flush();
			}
			return img;
		}

		/// <summary>
		/// Redimensiona de forma proporcional una dada imagen.
		/// </summary>
		/// <param name="source">La imagen para redimensionar.</param>
		/// <param name="maxWidth">El ancho maximo de la imagen resultante.</param>
		/// <param name="maxHeight">El alto maximo de la imagen resultante.</param>
		/// <returns>Una nueva instancia de imagen redimensionada de forma propocional a la original.</returns>
		/// <remarks>
		/// Siempre es una nueva instancia de <see cref="Image"/> de la cual se tendrá que hacer <see cref="Image.Dispose()"/>.
		/// </remarks>
		public static Image ResizeProportional(this Image source, int maxWidth, int maxHeight)
		{
			if (source == null)
			{
				return null;
			}
			int proportionalWidth;
			int proportionalHeight;
			if ((decimal)source.Width / source.Height > (decimal)maxWidth / maxHeight)
			{
				proportionalWidth = maxWidth;
				proportionalHeight = (source.Height * maxWidth) / source.Width;
			}
			else
			{
				proportionalWidth = (source.Width * maxHeight) / source.Height;
				proportionalHeight = maxHeight;
			}
			return source.Resize(proportionalWidth, proportionalHeight);
		}

		public static byte[] ToByteArray(this Image source)
		{
			return ToByteArray(source, ImageFormat.Jpeg);
		}

		public static byte[] ToByteArray(this Image source, ImageFormat format)
		{
			using (var ms = new MemoryStream())
			{
				source.Save(ms, format);
				return ms.ToArray();
			}
		}

		public static Image ToImage(this byte[] source)
		{
			using (var ms = new MemoryStream(source))
			{
				return Image.FromStream(ms);
			}
		}
	}
}
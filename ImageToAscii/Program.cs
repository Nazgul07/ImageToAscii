using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Console = Colorful.Console;

namespace ImageToAscii
{
	class Program
	{
		static void Main(string[] args)
		{
			args = new string[] { @"C:\Users\colto\Pictures\duck.png" };
			string imagePath;
			if(args.Length > 0 && File.Exists(imagePath = args[0]))
			{
				using (Bitmap image = new Bitmap(imagePath))
				{
					ImageConverter converter = new ImageConverter();
					double height = image.Height;
					if (height > 30)
					{
						height = 30;
					}
					double width = image.Width * (height / image.Height);
					width = width * 2;
					using (Bitmap resizedImage = ResizeImage(image, (int)Math.Round(width), (int)Math.Round(height)))
					{
						WriteImageToASCII(resizedImage);
					}
				}
			}
		}
		
		public static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
		
		public static void WriteImageToASCII(Bitmap img)
		{
			using(Bitmap lowbit = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed))
			{
				try
				{
					for (int y = 0; y < img.Height; y++)
					{
						for (int x = 0; x < img.Width; x++)
						{
							Color col = img.GetPixel(x, y);
							Color lowbitColor = lowbit.GetPixel(x, y);
							bool transparent = col.A == 0;
							col = Color.FromArgb((col.R + col.G + col.B) / 3,
								(col.R + col.G + col.B) / 3,
								(col.R + col.G + col.B) / 3);
							string character = GetCharacterByShade(col, transparent);
							Console.Write(character, col.R < 20 ? Color.White : lowbitColor);//black == white

							if (x == img.Width - 1)
								Console.WriteLine();
						}
					}
				}
				catch (Exception exc)
				{
					Console.WriteLine();
					Console.WriteLine(exc.ToString());
				}
			}
		}

		private const string BLACK = "@";
		private const string CHARCOAL = "#";
		private const string DARKGRAY = "8";
		private const string MEDIUMGRAY = "&";
		private const string MEDIUM = "?";
		private const string MEDIUMLOW = "o";
		private const string GRAY = ":";
		private const string SLATEGRAY = "*";
		private const string LIGHTSLATEGRAY = "-";
		private const string LIGHTGRAY = ".";
		private const string WHITE = " ";

		private static string GetCharacterByShade(Color col, bool transparent)
		{
			int redValue = int.Parse(col.R.ToString());
			string asciival = WHITE;
			if (redValue >= 230 || transparent)
			{
				asciival = WHITE;
			}
			else if (redValue >= 200)
			{
				asciival = LIGHTGRAY;
			}
			else if (redValue >= 180)
			{
				asciival = LIGHTSLATEGRAY;
			}
			else if (redValue >= 160)
			{
				asciival = SLATEGRAY;
			}
			else if (redValue >= 140)
			{
				asciival = GRAY;
			}
			else if (redValue >= 120)
			{
				asciival = MEDIUMLOW;
			}
			else if (redValue >= 100)
			{
				asciival = MEDIUM;
			}
			else if (redValue >= 70)
			{
				asciival = MEDIUMGRAY;
			}
			else if (redValue >= 45)
			{
				asciival = DARKGRAY;
			}
			else if (redValue >= 20)
			{
				asciival = CHARCOAL;
			}
			else
			{
				asciival = BLACK;
			}

			return asciival;
		}
	}
}

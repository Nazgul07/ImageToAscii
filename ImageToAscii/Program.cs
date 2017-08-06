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
			string imagePath;
			if(args.Length > 0 && File.Exists(imagePath = args[0]))
			{
				using (Bitmap image = new Bitmap(imagePath))
				{
					double height = image.Height > 30 ? 30 : image.Height;
					double width = image.Width * (height / image.Height) * 2;
					using (Bitmap resizedImage = ResizeImage(image, (int)Math.Round(width), (int)Math.Round(height)))
					{
						WriteImageToAscii(resizedImage);
					}
				}
			}
		}

		private static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (Graphics graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (ImageAttributes wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}
		
		private static void WriteImageToAscii(Bitmap img)
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
							char character = GetCharacterByShade(col, transparent);
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

		private const char Black = '@';
		private const char Charcoal = '#';
		private const char Darkgray = '8';
		private const char Mediumgray = '&';
		private const char Medium = '?';
		private const char Mediumlow = 'o';
		private const char Gray = ':';
		private const char Slategray = '*';
		private const char Lightslategray = '-';
		private const char Lightgray = '.';
		private const char White = ' ';

		private static char GetCharacterByShade(Color col, bool transparent)
		{
			var redValue = int.Parse(col.R.ToString());
			char asciichar;
			if (redValue >= 230 || transparent)
				asciichar = White;
			else if (redValue >= 200)
				asciichar = Lightgray;
			else if (redValue >= 180)
				asciichar = Lightslategray;
			else if (redValue >= 160)
				asciichar = Slategray;
			else if (redValue >= 140)
				asciichar = Gray;
			else if (redValue >= 120)
				asciichar = Mediumlow;
			else if (redValue >= 100)
				asciichar = Medium;
			else if (redValue >= 70)
				asciichar = Mediumgray;
			else if (redValue >= 45)
				asciichar = Darkgray;
			else if (redValue >= 20)
				asciichar = Charcoal;
			else
				asciichar = Black;
			return asciichar;
		}
	}
}

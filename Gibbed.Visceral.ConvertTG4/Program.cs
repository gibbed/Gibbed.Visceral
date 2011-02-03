using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gibbed.Visceral.ConvertTG4
{
    internal class Program
    {
        private static Bitmap MakeBitmapFromDXT(uint width, uint height, byte[] buffer, bool keepAlpha)
        {
            Bitmap bitmap = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);

            for (uint i = 0; i < width * height * 4; i += 4)
            {
                // flip red and blue
                byte r = buffer[i + 0];
                buffer[i + 0] = buffer[i + 2];
                buffer[i + 2] = r;
            }

            Rectangle area = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(area, ImageLockMode.WriteOnly, bitmap.PixelFormat);
            Marshal.Copy(buffer, 0, data.Scan0, (int)(width * height * 4));
            bitmap.UnlockBits(data);
            return bitmap;
        }

        public static void Main(string[] args)
        {
            byte[] data;
            using (var input = File.OpenRead("eurostileltstdbold32.tg4d"))
            {
                data = new byte[input.Length];
                input.Read(data, 0, data.Length);
            }

            var data2 = Gibbed.Squish.Native.DecompressImage(
                data,
                0x0400, 0x0200,
                (int)Gibbed.Squish.Native.SquishFlags.DXT5);

            var bitmap = MakeBitmapFromDXT(0x0400, 0x0200, data2, true);
            bitmap.Save("eurostileltstdbold32.png");
        }
    }
}

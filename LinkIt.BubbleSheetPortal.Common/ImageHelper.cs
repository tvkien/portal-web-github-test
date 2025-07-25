using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class ImageHelper
    {
        public static List<Image> ConvertByteArraysToImages(List<byte[]> byteArrays)
        {
            var images = new List<Image>();

            foreach (var byteArray in byteArrays)
            {
                using (var ms = new MemoryStream(byteArray))
                {
                    images.Add(Image.FromStream(ms));
                }
            }

            return images;
        }

        public static Image CombineImagesVertically(List<Image> images)
        {
            if (images == null || images.Count == 0)
                return null;

            int width = 0;
            int height = 0;

            foreach (var img in images)
            {
                width = Math.Max(width, img.Width);
                height += img.Height;
            }

            Bitmap finalImage = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(finalImage))
            {
                g.Clear(Color.White);
                int offsetY = 0;

                foreach (var img in images)
                {
                    g.DrawImage(img, 0, offsetY);
                    offsetY += img.Height;
                }
            }

            return finalImage;
        }
    }
}

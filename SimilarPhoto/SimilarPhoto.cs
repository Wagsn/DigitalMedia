using System;
using System.Drawing;

namespace SimilarPhoto
{
    /// <summary>
    /// 相似照片
    /// </summary>
    internal class SimilarPhoto
    {
        private readonly string FilePath;

        public SimilarPhoto(string filePath)
        {
            FilePath = filePath;
        }


        // 平均HASH
        public string GetHash()
        {
            Image image = ReduceSize(FilePath);
            byte[] grayValues = ReduceColor(image);
            byte average = CalcAverage(grayValues);
            string reslut = ComputeBits(grayValues, average);
            return reslut;
        }

        // Step 1 : Reduce size to 8*8
        private static Image ReduceSize(string filePath, int width = 8, int height = 8)
        {
            using (var source = Image.FromFile(filePath))
            {
                Image image = source.GetThumbnailImage(width, height, () => { return false; }, IntPtr.Zero);
                return image;
            }
        }

        // Step 2 : Reduce Color
        private static byte[] ReduceColor(Image image)
        {
            Bitmap bitMap = new Bitmap(image);
            byte[] grayValues = new byte[image.Width * image.Height];

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color color = bitMap.GetPixel(x, y);
                    byte grayValue = (byte)((color.R * 30 + color.G * 59 + color.B * 11) / 100);
                    grayValues[x * image.Width + y] = grayValue;
                }
            }

            return grayValues;
        }

        // Step 3 : Average the colors
        private static byte CalcAverage(byte[] values)
        {
            int sum = 0;
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }

            return Convert.ToByte(sum / values.Length);
        }

        // Step 4 : Compute the bits
        private static string ComputeBits(byte[] values, byte averageValue)
        {
            char[] result = new char[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] < averageValue)
                {
                    result[i] = '0';
                }
                else
                {
                    result[i] = '1';
                }
            }
            return new string(result);
        }

        // Compare hash
        public static int CalcSimilarDegree(string a, string b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException();
            }

            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i])
                {
                    count++;
                }
            }
            return count;
        }
    }
}
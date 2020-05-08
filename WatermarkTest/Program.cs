using System;

namespace WatermarkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(", ", args));

            Console.WriteLine("Hello World!");

            var srcImgPath = @"C:\Users\wagsn\Desktop\2018.10_21.jpg";
            var mrkImgPath = @"C:\Users\wagsn\Desktop\adfd446df777490004e3de9bf154bb59.jpg";
            var mrkText = "Wagsn";
            var dstImgPath = @"C:\Users\wagsn\Desktop\dst.jpg";

            // 大小无法自适应
            new Watermark.WatermarkMaker().BuildWatermark(srcImgPath, mrkImgPath, mrkText, dstImgPath);
        }
    }
}

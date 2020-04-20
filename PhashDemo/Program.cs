using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;
using Shipwreck.Phash.Imaging;

namespace PhashDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // 较大规模图片 使用phash去重 https://www.jianshu.com/p/c87f6f69d51f
            var fullPathToImage = Path.Combine(AppContext.BaseDirectory,"5e65b93129529.jpg");
            var bitmap = (Bitmap)Image.FromFile(fullPathToImage);
            var hash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());

            // 5e65b92cef353.jpg
            var fullPathToImage2 = Path.Combine(AppContext.BaseDirectory, "5e65b92cef353.jpg");
            var bitmap2 = (Bitmap)Image.FromFile(fullPathToImage2);
            var hash2 = ImagePhash.ComputeDigest(bitmap2.ToLuminanceImage());

            Console.WriteLine(@"E:\WorkSpace\GitHub\DigitalMedia\PhashDemo\bin\Debug\netcoreapp3.1\5e65b93129529.jpg, " + hash.Coefficients.Length +", "+ string.Join("", hash.Coefficients.Select(a => a.ToString())));

            var score = ImagePhash.GetCrossCorrelation(hash, hash2);
            Console.WriteLine(score);
        }

        // Multithreaded hashing of all images in a folder
        public static (ConcurrentDictionary<string, Digest> filePathsToHashes, ConcurrentDictionary<Digest, HashSet<string>> hashesToFiles) GetHashes(string dirPath, string searchPattern)
        {
            var filePathsToHashes = new ConcurrentDictionary<string, Digest>();
            var hashesToFiles = new ConcurrentDictionary<Digest, HashSet<string>>();

            var files = Directory.GetFiles(dirPath, searchPattern);

            Parallel.ForEach(files, (currentFile) =>
            {
                var bitmap = (Bitmap)Image.FromFile(currentFile);
                var hash = ImagePhash.ComputeDigest(bitmap.ToLuminanceImage());
                filePathsToHashes[currentFile] = hash;

                HashSet<string> currentFilesForHash;

                lock (hashesToFiles)
                {
                    if (!hashesToFiles.TryGetValue(hash, out currentFilesForHash))
                    {
                        currentFilesForHash = new HashSet<string>();
                        hashesToFiles[hash] = currentFilesForHash;
                    }
                }

                lock (currentFilesForHash)
                {
                    currentFilesForHash.Add(currentFile);
                }
            });

            return (filePathsToHashes, hashesToFiles);
        }
    }
}

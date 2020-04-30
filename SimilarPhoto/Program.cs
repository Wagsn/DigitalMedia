using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimilarPhoto
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("输入文件夹：");
            // 计算HASH
            var folder = Console.ReadLine();
            var files = Directory.EnumerateFiles(folder).Where(a => Regex.IsMatch(a, @"\.((jpg)|(jpeg)|(png))$"));
            Dictionary<string, string> pathHashs = new Dictionary<string, string>();
            foreach(var file in files)
            {
                var similar = new SimilarPhoto(file);
                var hash = similar.GetHash();
                pathHashs[file] = hash;
                Console.WriteLine(hash + " : " + Path.GetFileName(file));
            }

            Console.WriteLine("====================================");

            // 每个文件和其它所有文件的HASH比较，取最相似的前几个
            var res = pathHashs.Select(ph =>
            {
                var result = pathHashs.Keys.Where(a => a != ph.Key).Select(a =>
                {
                    return new
                    {
                        Key = a,
                        Value = pathHashs[a],
                        Degree = (int?)SimilarPhoto.CalcSimilarDegree(pathHashs[ph.Key], pathHashs[a])
                    };
                }).Where(a => a.Degree > 50).OrderByDescending(a => a.Degree).Take(5).ToList();
                return new
                {
                    Key = ph.Key,
                    Value = ph.Value,
                    Max = result.Max(a => a.Degree),
                    Similes = result,
                };
            }).OrderByDescending(a => a.Max).ToList();
            foreach(var ph in res)
            {
                Console.WriteLine(ph.Value + " : " + ph.Key);
                foreach(var item in ph.Similes)
                {
                    Console.WriteLine("  " + ((item.Degree??0)/(double)64).ToString("0.0000") + " " + item.Key);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Gibbed.Visceral.FileFormats;

namespace Gibbed.Visceral.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /*
            using (var input = File.Open(
                @"T:\Games\EADM\Dead Space™ 2\DS2DAT1.DAT",
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite))
            {
                var big = new BigFile();
                big.Deserialize(input);
            }
            */

            //var test1 = @"buttons".HashName().ToString("X8");
            //var test2 = @"shared\fonts\buttons.tg4h".HashName().ToString("X8");
            //var test3 = @"tg4h".HashName().ToString("X8");
            var test4 = @"CMD_SetMovieSubdir".HashName().ToString("X8");

            /*
            using (var input = File.Open(
                @"T:\DS2\data0\text_assets\text_assets_global.str",
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var set = new StreamSetFile();
                set.Deserialize(input);
            }
            */

            /*
            foreach (var path in Directory.GetFiles(@"T:\DS2\data6", "*.str", SearchOption.AllDirectories))
            {
                Console.WriteLine(path);

                using (var input = File.Open(
                    path,
                    FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var set = new StreamSetFile();
                    set.Deserialize(input);
                }
            }
            */

            /*
            using (var input = File.OpenRead("test.ref"))
            {
                var test = RefPack.Decompression.Decompress(input);
            }
            */

            var prefix = @"T:\DI\bigfile1";
            int i = 0;
            foreach (var path in Directory.GetFiles(prefix, "*.str", SearchOption.AllDirectories))
            {
                var newPath = Path.GetFileNameWithoutExtension(path);
                newPath = string.Format("{0}_{1}", i, newPath);
                newPath = Path.Combine(prefix + "_unpacked", newPath);

                UnpackSTR.Program.Main(new string[] { path,  newPath });

                i++;
            }
        }
    }
}

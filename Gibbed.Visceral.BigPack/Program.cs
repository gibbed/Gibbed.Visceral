/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.Helpers;
using Gibbed.Visceral.FileFormats;
using NDesk.Options;

namespace Gibbed.Visceral.BigPack
{
    internal class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool showHelp = false;
            bool verbose = false;

            OptionSet options = new OptionSet()
            {
                {
                    "v|verbose",
                    "be verbose (list files)",
                    v => verbose = v != null
                },
                {
                    "h|help",
                    "show this message and exit", 
                    v => showHelp = v != null
                },
            };

            List<string> extras;

            try
            {
                extras = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extras.Count < 1 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ output_big input_directory+", GetExecutableName());
                Console.WriteLine("Pack files from input directories into a Big File.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            var inputPaths = new List<string>();
            string outputPath;

            if (extras.Count == 1)
            {
                inputPaths.Add(extras[0]);
                outputPath = Path.ChangeExtension(extras[0], ".viv");
            }
            else
            {
                outputPath = extras[0];
                inputPaths.AddRange(extras.Skip(1));
            }

            var paths = new SortedDictionary<uint, string>();

            if (verbose == true)
            {
                Console.WriteLine("Finding files...");
            }

            foreach (var relPath in inputPaths)
            {
                string inputPath = Path.GetFullPath(relPath);

                if (inputPath.EndsWith(Path.DirectorySeparatorChar.ToString()) == true)
                {
                    inputPath = inputPath.Substring(0, inputPath.Length - 1);
                }

                foreach (string path in Directory.GetFiles(inputPath, "*", SearchOption.AllDirectories))
                {
                    string fullPath = Path.GetFullPath(path);
                    string partPath = fullPath.Substring(inputPath.Length + 1).ToLowerInvariant();

                    uint hash = 0xFFFFFFFF;
                    if (partPath.ToUpper().StartsWith("__UNKNOWN") == true)
                    {
                        string partName;
                        
                        partName = Path.GetFileNameWithoutExtension(partPath);
                        if (partName.Length > 8)
                        {
                            partName = partName.Substring(0, 8);
                        }

                        hash = uint.Parse(
                            partName,
                            System.Globalization.NumberStyles.AllowHexSpecifier);
                    }
                    else
                    {
                        hash = partPath.ToLowerInvariant().HashFileName();
                    }

                    if (paths.ContainsKey(hash) == true)
                    {
                        Console.WriteLine("Ignoring {0} duplicate.", partPath);
                        continue;
                    }

                    paths[hash] = fullPath;
                }
            }

            using (var output = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var big = new BigFile();

                if (verbose == true)
                {
                    Console.WriteLine("Adding files...");
                }

                // write a dummy header
                output.Seek(0, SeekOrigin.Begin);
                big.Entries.Clear();
                foreach (var kvp in paths)
                {
                    big.Entries.Add(new BigFile.Entry()
                        {
                            Name = 0,
                            Offset = 0,
                            Size = 0,
                        });
                }
                big.Serialize(output);

                output.Seek(output.Position.Align(2048), SeekOrigin.Begin);
                long baseOffset = output.Position;

                // write file data
                big.Entries.Clear();

                if (verbose == true)
                {
                    Console.WriteLine("Writing to disk...");
                }

                foreach (var kvp in paths)
                {
                    if (verbose == true)
                    {
                        Console.WriteLine(kvp.Value);
                    }

                    using (var input = File.Open(kvp.Value, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        output.Seek(baseOffset, SeekOrigin.Begin);

                        uint size = (uint)input.Length.Align(2048);

                        big.Entries.Add(new BigFile.Entry()
                            {
                                Name = kvp.Key,
                                Offset = (uint)output.Position,
                                Size = size,
                            });

                        output.WriteFromStream(input, input.Length);
                        baseOffset += size;
                    }
                }

                // write filled header
                output.Seek(0, SeekOrigin.Begin);
                big.TotalFileSize = (uint)output.Length;
                big.Serialize(output);
            }
        }
    }
}

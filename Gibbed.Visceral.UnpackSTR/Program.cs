using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.Helpers;
using Gibbed.Visceral.FileFormats;
using NDesk.Options;
using StreamSet = Gibbed.Visceral.FileFormats.StreamSet;

namespace Gibbed.Visceral.UnpackSTR
{
    public class Program
    {
        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }

        public static void Main(string[] args)
        {
            bool verbose = false;
            bool overwriteFiles = false;
            bool showHelp = false;

            OptionSet options = new OptionSet()
            {
                {
                    "v|verbose",
                    "be verbose (list files)",
                    v => verbose = v != null
                },
                {
                    "o|overwrite",
                    "overwrite files if they already exist", 
                    v => overwriteFiles = v != null
                },
                {
                    "h|help",
                    "show this message and exit", 
                    v => showHelp = v != null
                },
            };

            List<string> extra;

            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("{0}: ", GetExecutableName());
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `{0} --help' for more information.", GetExecutableName());
                return;
            }

            if (extra.Count < 1 || extra.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_sarc [output_directory]", GetExecutableName());
                Console.WriteLine("Unpack specified small archive.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string inputPath = extra[0];
            string outputPath = extra.Count > 1 ? extra[1] : Path.ChangeExtension(inputPath, null) + "_unpacked";

            Stream input = File.OpenRead(inputPath);
            TextWriter writer;

            Directory.CreateDirectory(outputPath);

            // hacky solution to the really long filenames streamsets can contain, for now
            writer = new StreamWriter(Path.Combine(outputPath, "__REMAPPED__.txt"));

            var streamSet = new StreamSetFile();
            streamSet.Deserialize(input);

            for (int i = 0; i < streamSet.Contents.Count; )
            {
                var headerInfo = streamSet.Contents[i];
                if (headerInfo.Type != StreamSet.ContentType.Header)
                {
                    throw new InvalidOperationException();
                }

                input.Seek(headerInfo.Offset, SeekOrigin.Begin);

                uint unknown00 = input.ReadValueU32(streamSet.LittleEndian);
                ushort unknown04 = input.ReadValueU16(streamSet.LittleEndian);
                ushort unknown06 = input.ReadValueU16(streamSet.LittleEndian);

                uint unknown08 = input.ReadValueU32(streamSet.LittleEndian);

                uint unknown0C = input.ReadValueU32(streamSet.LittleEndian);
                uint unknown10 = input.ReadValueU32(streamSet.LittleEndian);
                uint unknown14 = input.ReadValueU32(streamSet.LittleEndian);
                uint unknown18 = input.ReadValueU32(streamSet.LittleEndian);

                uint totalSize = input.ReadValueU32(streamSet.LittleEndian);

                string baseName = input.ReadStringZ();
                string fileName = input.ReadStringZ();
                string fileType = input.ReadStringZ();

                if (input.Position > headerInfo.Offset + headerInfo.Size)
                {
                    throw new InvalidOperationException();
                }

                i++;

                Console.WriteLine("{0}", fileName);

                if (fileName.Length > 100)
                {
                    if (baseName.Length > 50)
                    {
                        baseName = baseName.Substring(0, 50);
                    }

                    var newName = Path.Combine(
                        "__REMAPPED__",
                        string.Format("{0}_{1}.{2}", i, baseName, fileType));

                    writer.WriteLine("{0} => {1}",
                        newName, fileName);

                    fileName = newName;
                }

                string outputName = Path.Combine(outputPath, fileName);

                if (overwriteFiles == false &&
                    File.Exists(outputName) == true)
                {
                    continue;
                }

                var dirName = Path.GetDirectoryName(outputName);
                Directory.CreateDirectory(dirName);

                using (var output = File.Create(outputName))
                {
                    uint readSize = 0;
                    while (readSize < totalSize)
                    {
                        uint leftSize = totalSize - readSize;

                        var dataInfo = streamSet.Contents[i];
                        if (dataInfo.Type != StreamSet.ContentType.Data &&
                            dataInfo.Type != StreamSet.ContentType.CompressedData)
                        {
                            throw new InvalidOperationException();
                        }

                        input.Seek(dataInfo.Offset, SeekOrigin.Begin);

                        if (dataInfo.Type == StreamSet.ContentType.CompressedData)
                        {
                            var compressedSize = input.ReadValueU32(streamSet.LittleEndian);
                            if (4 + compressedSize > dataInfo.Size)
                            {
                                throw new InvalidOperationException();
                            }

                            var compressedStream = input.ReadToMemoryStream(compressedSize);
                            var compressedData = Gibbed.RefPack.Decompression.Decompress(
                                compressedStream);

                            uint writeSize = Math.Min(leftSize, (uint)compressedData.Length);
                            output.Write(compressedData, 0, (int)writeSize);
                            readSize += writeSize;
                        }
                        else
                        {
                            uint writeSize = Math.Min(leftSize, dataInfo.Size);
                            output.WriteFromStream(input, writeSize);
                            readSize += writeSize;
                        }

                        ++i;
                    }
                }
            }

            writer.Flush();
            writer.Close();
        }
    }
}

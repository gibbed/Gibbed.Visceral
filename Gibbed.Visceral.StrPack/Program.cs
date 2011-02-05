using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.XPath;
using Gibbed.Helpers;
using NDesk.Options;
using StreamSet = Gibbed.Visceral.FileFormats.StreamSet;

namespace Gibbed.Visceral.StrPack
{
    public class Program
    {
        private static void Fill(MemoryStream output)
        {
            if (output.Position < output.Capacity)
            {
                if (output.Position + 8 > output.Capacity)
                {
                    throw new InvalidOperationException();
                }

                uint size = (uint)(output.Capacity - output.Position);
                output.WriteValueU32((uint)StreamSet.BlockType.Padding);
                output.WriteValueU32(size);
                output.SetLength(output.Capacity);
            }
        }

        public static void Main(string[] args)
        {
            bool verbose = false;
            bool showHelp = false;

            OptionSet options = new OptionSet()
            {
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

            if (extras.Count < 1 || extras.Count > 2 || showHelp == true)
            {
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_directory [output_str]", GetExecutableName());
                Console.WriteLine("Pack directory into a stream set.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string inputPath = extras[0];
            string outputPath = extras.Count > 1 ? extras[1] : Path.ChangeExtension(inputPath, ".str");

            if (Directory.Exists(inputPath) == true)
            {
                string testPath = Path.Combine(inputPath, "@archive.xml");
                if (File.Exists(testPath) == true)
                {
                    inputPath = testPath;
                }
            }

            var streams = new List<MyFileInfo>();

            using (var input = File.Open(inputPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var doc = new XPathDocument(input);
                var nav = doc.CreateNavigator();

                var root = nav.SelectSingleNode("/streams");

                var nodes = root.Select("stream");
                while (nodes.MoveNext() == true)
                {
                    var node = nodes.Current;
                    var stream = new MyFileInfo();

                    var build = node.GetAttribute("build", "");
                    if (Enum.TryParse<StreamSet.FileBuild>(build, out stream.Build) == false)
                    {
                        stream.Build = (StreamSet.FileBuild)uint.Parse(build, NumberStyles.AllowHexSpecifier);
                    }

                    stream.Alignment = ushort.Parse(node.GetAttribute("alignment", ""), NumberStyles.AllowHexSpecifier);
                    stream.Flags = ushort.Parse(node.GetAttribute("flags", ""), NumberStyles.AllowHexSpecifier);
                    
                    stream.Type = uint.Parse(node.GetAttribute("type", ""), NumberStyles.AllowHexSpecifier);

                    stream.Unknown0C = uint.Parse(node.GetAttribute("u0C", ""), NumberStyles.AllowHexSpecifier);
                    stream.Type2 = uint.Parse(node.GetAttribute("type2", ""), NumberStyles.AllowHexSpecifier);
                    stream.Unknown14 = uint.Parse(node.GetAttribute("u14", ""), NumberStyles.AllowHexSpecifier);
                    stream.Unknown18 = uint.Parse(node.GetAttribute("u18", ""), NumberStyles.AllowHexSpecifier);

                    stream.BaseName = node.GetAttribute("base_name", "");
                    stream.FileName = node.GetAttribute("file_name", "");
                    stream.TypeName = node.GetAttribute("type_name", "");

                    string path = node.Value;

                    if (Path.IsPathRooted(path) == false)
                    {
                        path = Path.Combine(Path.GetDirectoryName(inputPath), path);
                        path = Path.GetFullPath(path);
                    }

                    stream.Path = path;
                    streams.Add(stream);
                }
            }

            using (var output = File.Open(
                outputPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.ReadWrite))
            {
                var buffer = new MemoryStream(0x00020000);

                buffer.WriteValueU32((uint)StreamSet.BlockType.Options);
                buffer.WriteValueU32(12);
                buffer.WriteValueU16(2);
                buffer.WriteValueU16(259);

                foreach (var stream in streams)
                {
                    using (var input = File.OpenRead(stream.Path))
                    {
                        uint totalSize = (uint)input.Length;
                        stream.TotalSize = totalSize;

                        var info = new MemoryStream();
                        info.WriteValueU32((uint)StreamSet.ContentType.Header);
                        stream.Serialize(info, true);
                        info.SetLength(info.Length.Align(4));
                        info.Position = 0;

                        if (buffer.Position + (8 + info.Length) > buffer.Capacity)
                        {
                            Fill(buffer);
                            buffer.Position = 0;
                            output.WriteFromStream(buffer, buffer.Length);
                            buffer.SetLength(0);
                        }

                        buffer.WriteValueU32((uint)StreamSet.BlockType.Content);
                        buffer.WriteValueU32((uint)(8 + info.Length));
                        buffer.WriteFromStream(info, info.Length);

                        uint leftSize = totalSize;
                        while (leftSize > 0)
                        {
                            uint blockSize = leftSize;

                            if ((buffer.Capacity - buffer.Position) > 12)
                            {
                                blockSize = (uint)Math.Min(leftSize, (buffer.Capacity - buffer.Position) - 12);
                            }
                            else
                            {
                                blockSize = (uint)Math.Min(leftSize, buffer.Capacity - 12);
                            }

                            var data = new MemoryStream();
                            data.WriteValueU32((uint)StreamSet.ContentType.Data);
                            data.WriteFromStream(input, blockSize);
                            data.Position = 0;

                            if (buffer.Position + (8 + data.Length) > buffer.Capacity)
                            {
                                Fill(buffer);
                                buffer.Position = 0;
                                output.WriteFromStream(buffer, buffer.Length);
                                buffer.SetLength(0);
                            }

                            buffer.WriteValueU32((uint)StreamSet.BlockType.Content);
                            buffer.WriteValueU32((uint)(8 + data.Length));
                            buffer.WriteFromStream(data, data.Length);

                            leftSize -= blockSize;
                        }
                    }
                }

                Fill(buffer);
                buffer.Position = 0;

                if (buffer.Length > 0)
                {
                    output.WriteFromStream(buffer, buffer.Length);
                }
            }
        }

        private static string GetExecutableName()
        {
            return Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        }
    }
}

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
using System.Xml;
using Gibbed.Helpers;
using Gibbed.Visceral.FileFormats;
using NDesk.Options;
using StreamSet = Gibbed.Visceral.FileFormats.StreamSet;

namespace Gibbed.Visceral.StrUnpack
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
            bool overwriteFiles = true;
            bool showHelp = false;
            bool debugMode = false;

            OptionSet options = new OptionSet()
            {
                {
                    "v|verbose",
                    "be verbose (list files)",
                    v => verbose = v != null
                },
                {
                    "d|debug",
                    "debug mode",
                    v => debugMode = v != null
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
                Console.WriteLine("Usage: {0} [OPTIONS]+ input_str [output_directory]", GetExecutableName());
                Console.WriteLine("Unpack specified stream set.");
                Console.WriteLine();
                Console.WriteLine("Options:");
                options.WriteOptionDescriptions(Console.Out);
                return;
            }

            string inputPath = extra[0];
            string outputPath = extra.Count > 1 ? extra[1] : Path.ChangeExtension(inputPath, null) + "_unpacked";

            Stream input = File.OpenRead(inputPath);
            Directory.CreateDirectory(outputPath);

            var settings = new XmlWriterSettings();
            settings.Indent = true;

            using (var xml = XmlWriter.Create(
                Path.Combine(outputPath, "@archive.xml"), settings))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("streams");

                var set = new StreamSetFile();
                set.Deserialize(input);

                int counter = 0;
                for (int i = 0; i < set.Contents.Count; )
                {
                    var headerInfo = set.Contents[i];
                    if (headerInfo.Type != StreamSet.ContentType.Header)
                    {
                        //throw new FormatException("excepted header");
                        i++;
                        continue;
                    }

                    input.Seek(headerInfo.Offset, SeekOrigin.Begin);

                    var fileInfo = new StreamSet.FileInfo();
                    fileInfo.Deserialize(input, set.LittleEndian);

                    if (input.Position > headerInfo.Offset + headerInfo.Size)
                    {
                        throw new FormatException("read too much header data?");
                    }

                    /*
                    if (Enum.IsDefined(typeof(StreamSet.FileBuild), fileInfo.Build) == false)
                    {
                        throw new FormatException("unsupported build type " + ((uint)fileInfo.Build).ToString("X8"));
                    }*/
                    /*else if (fileInfo.Type != fileInfo.Type2)
                    {
                        throw new FormatException("type hashes don't match");
                    }
                    else if (fileInfo.Type != fileInfo.TypeName.HashFileName())
                    {
                        throw new FormatException("type name hash and type hash don't match");
                    }*/

                    string fileName;

                    fileName =
                        counter.ToString("D4") + "_" +
                        fileInfo.GetSaneFileName();
                    counter++;

                    fileName =
                        Path.Combine(fileInfo.TypeName, fileName);

                    i++;

                    Console.WriteLine("{0}", fileInfo.FileName);

                    string outputName = Path.Combine(outputPath, fileName);

                    if (overwriteFiles == false &&
                        File.Exists(outputName) == true)
                    {
                        continue;
                    }

                    xml.WriteStartElement("stream");
                    
                    //xml.WriteAttributeString("build", fileInfo.Build.ToString());

                    if (Enum.IsDefined(typeof(StreamSet.FileBuild), fileInfo.Build) == false)
                    {
                        xml.WriteAttributeString("build", ((uint)fileInfo.Build).ToString("X8"));
                    }
                    else
                    {
                        xml.WriteAttributeString("build", fileInfo.Build.ToString());
                    }
                    
                    xml.WriteAttributeString("alignment", fileInfo.Alignment.ToString("X4"));
                    xml.WriteAttributeString("flags", fileInfo.Flags.ToString("X4"));
                    xml.WriteAttributeString("type", fileInfo.Type.ToString("X8"));
                    xml.WriteAttributeString("u0C", fileInfo.Unknown0C.ToString("X8"));
                    xml.WriteAttributeString("type2", fileInfo.Type2.ToString("X8"));
                    xml.WriteAttributeString("u14", fileInfo.Unknown14.ToString("X8"));
                    xml.WriteAttributeString("u18", fileInfo.Unknown18.ToString("X8"));
                    xml.WriteAttributeString("base_name", fileInfo.BaseName);
                    xml.WriteAttributeString("file_name", fileInfo.FileName);
                    xml.WriteAttributeString("type_name", fileInfo.TypeName);

                    if (debugMode == false)
                    {
                        xml.WriteString(fileName);
                    }

                    if (debugMode == true)
                    {
                        xml.WriteStartElement("blocks");
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(outputName));

                    using (var output = File.Create(outputName))
                    {
                        uint readSize = 0;
                        while (readSize < fileInfo.TotalSize)
                        {
                            uint leftSize = fileInfo.TotalSize - readSize;

                            var dataInfo = set.Contents[i];
                            if (dataInfo.Type != StreamSet.ContentType.Data &&
                                dataInfo.Type != StreamSet.ContentType.CompressedData)
                            {
                                throw new InvalidOperationException();
                            }

                            input.Seek(dataInfo.Offset, SeekOrigin.Begin);

                            if (debugMode == true)
                            {
                                xml.WriteStartElement("block");
                            }

                            if (dataInfo.Type == StreamSet.ContentType.CompressedData)
                            {
                                if (debugMode == true)
                                {
                                    xml.WriteAttributeString("type", "compressed");
                                }

                                var compressedSize = input.ReadValueU32(set.LittleEndian);
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

                                if (debugMode == true)
                                {
                                    xml.WriteAttributeString("offset", dataInfo.Offset.ToString("X8"));
                                    xml.WriteAttributeString("tsize", (dataInfo.Size + 12).ToString());
                                    xml.WriteAttributeString("size", dataInfo.Size.ToString());
                                    xml.WriteAttributeString("csize", compressedSize.ToString());
                                    xml.WriteAttributeString("usize1", writeSize.ToString());
                                    xml.WriteAttributeString("usize2", compressedData.Length.ToString());
                                }
                            }
                            else
                            {
                                if (debugMode == true)
                                {
                                    xml.WriteAttributeString("type", "uncompressed");
                                }

                                uint writeSize = Math.Min(leftSize, dataInfo.Size);
                                output.WriteFromStream(input, writeSize);
                                readSize += writeSize;

                                if (debugMode == true)
                                {
                                    xml.WriteAttributeString("offset", dataInfo.Offset.ToString("X8"));
                                    xml.WriteAttributeString("tsize", (dataInfo.Size + 12).ToString());
                                    xml.WriteAttributeString("size", dataInfo.Size.ToString());
                                }
                            }

                            if (debugMode == true)
                            {
                                xml.WriteEndElement();
                            }

                            ++i;
                        }
                    }

                    if (debugMode == true)
                    {
                        xml.WriteEndElement();
                    }
                    
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
            }
        }
    }
}

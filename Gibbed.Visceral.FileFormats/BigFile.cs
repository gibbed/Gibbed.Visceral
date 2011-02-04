using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Visceral.FileFormats
{
    public class BigFile
    {
        public struct Entry
        {
            public uint Offset;
            public uint Size;
            public uint Name;
            public bool Duplicate;
        }

        public List<Entry> Entries = new List<Entry>();
        public uint TotalFileSize;

        public void Serialize(Stream output)
        {
            output.WriteValueU32(0x42494748, false);
            output.WriteValueU32(this.TotalFileSize, true);
            output.WriteValueS32(this.Entries.Count, false);
            output.WriteValueS32(16 + (this.Entries.Count * 12) + 8, false);

            foreach (var entry in this.Entries)
            {
                output.WriteValueU32(entry.Offset, false);
                output.WriteValueU32(entry.Size, false);
                output.WriteValueU32(entry.Name, false);
            }

            output.WriteValueU32(0x4C323833, false);
            output.WriteValueU32(0x15050000, false);
        }

        public void Deserialize(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);

            var magic = input.ReadValueU32(false);
            if (magic != 0x42494748) // BIGH
            {
                throw new FormatException("unknown magic");
            }

            this.TotalFileSize = input.ReadValueU32(true); // :wtc:
            uint fileCount = input.ReadValueU32(false);
            uint headerSize = input.ReadValueU32(false);

            this.Entries.Clear();
            var duplicateNames = new List<uint>();
            for (uint i = 0; i < fileCount; i++)
            {
                var entry = new Entry();
                entry.Offset = input.ReadValueU32(false);
                entry.Size = input.ReadValueU32(false);
                entry.Name = input.ReadValueU32(false);

                if (duplicateNames.Contains(entry.Name) == true)
                {
                    entry.Duplicate = true;
                }
                else
                {
                    entry.Duplicate = false;
                    duplicateNames.Add(entry.Name);
                }

                this.Entries.Add(entry);
            }
        }
    }
}

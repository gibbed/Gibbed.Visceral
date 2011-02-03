using System;
using System.Collections;
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

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);

            var magic = input.ReadValueU32(false);
            if (magic != 0x42494748) // BIGH
            {
                throw new FormatException("unknown magic");
            }

            uint totalFileSize = input.ReadValueU32(true); // :wtc:
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

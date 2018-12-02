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
using Gibbed.IO;

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
            const Endian endian = Endian.Big;

            output.WriteValueU32(0x42494748, endian);
            output.WriteValueU32(this.TotalFileSize, Endian.Little);
            output.WriteValueS32(this.Entries.Count, endian);
            output.WriteValueS32(16 + (this.Entries.Count * 12) + 8, endian);

            foreach (var entry in this.Entries)
            {
                output.WriteValueU32(entry.Offset, endian);
                output.WriteValueU32(entry.Size, endian);
                output.WriteValueU32(entry.Name, endian);
            }

            output.WriteValueU32(0x4C323833, endian);
            output.WriteValueU32(0x15050000, endian);
        }

        public void Deserialize(Stream input)
        {
            const Endian endian = Endian.Big;

            input.Seek(0, SeekOrigin.Begin);

            var magic = input.ReadValueU32(endian);
            if (magic != 0x42494748) // BIGH
            {
                throw new FormatException("unknown magic");
            }

            this.TotalFileSize = input.ReadValueU32(Endian.Little); // :wtc:
            uint fileCount = input.ReadValueU32(endian);
            uint headerSize = input.ReadValueU32(endian);

            this.Entries.Clear();
            var duplicateNames = new List<uint>();
            for (uint i = 0; i < fileCount; i++)
            {
                var entry = new Entry();
                entry.Offset = input.ReadValueU32(endian);
                entry.Size = input.ReadValueU32(endian);
                entry.Name = input.ReadValueU32(endian);

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

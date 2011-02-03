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
        }

        private List<uint> _Keys = new List<uint>();
        private Hashtable Entries = new Hashtable();

        public IEnumerable<uint> Keys
        {
            get { return this._Keys; }
        }

        public void Serialize(Stream output)
        {
            throw new NotImplementedException();
        }

        public bool Contains(uint key)
        {
            return this.Entries.ContainsKey(key);
        }

        public Entry Get(uint key)
        {
            if (this.Entries.ContainsKey(key) == false)
            {
                throw new KeyNotFoundException();
            }

            return (Entry)this.Entries[key];
        }

        public void Set(uint key, Entry entry)
        {
            this.Entries[key] = entry;
        }

        public Entry this[uint key]
        {
            get
            {
                return this.Get(key);
            }
            set
            {
                this.Set(key, value);
            }
        }

        public void Set(uint key, uint offset, uint size)
        {
            this.Set(key, new Entry()
            {
                Offset = offset,
                Size = size,
            });
        }

        public void Remove(uint key)
        {
            if (this.Entries.ContainsKey(key) == false)
            {
                throw new KeyNotFoundException();
            }

            this.Entries.Remove(key);
        }

        public void Move(uint oldKey, uint newKey)
        {
            if (this.Entries.ContainsKey(oldKey) == false)
            {
                throw new KeyNotFoundException();
            }
            else if (this.Entries.ContainsKey(newKey) == true)
            {
                throw new ArgumentException("already contains the new key", "newKey");
            }

            this.Entries[newKey] = this.Entries[oldKey];
            this.Entries.Remove(oldKey);
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
            this._Keys.Clear();

            for (uint i = 0; i < fileCount; i++)
            {
                Entry entry = new Entry();
                entry.Offset = input.ReadValueU32(false);
                entry.Size = input.ReadValueU32(false);
                uint name = input.ReadValueU32(false);

                if (this.Entries.ContainsKey(name) == true)
                {
                    if (entry.Size != ((Entry)this.Entries[name]).Size)
                    {
                        throw new InvalidCastException();
                    }

                    continue;
                }

                this._Keys.Add(name);
                this.Entries.Add(name, entry);
            }
        }
    }
}

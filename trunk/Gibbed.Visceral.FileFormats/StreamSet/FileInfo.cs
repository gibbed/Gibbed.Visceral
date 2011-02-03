using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gibbed.Helpers;
using System.IO;

namespace Gibbed.Visceral.FileFormats.StreamSet
{
    public class FileInfo
    {
        public FileBuild Build;
        public ushort Unknown04; // flags?
        public ushort Unknown06; // flags?
        
        public uint Type;

        public uint Unknown0C;
        public uint Type2;
        public uint Unknown14;
        public uint Unknown18; // seems to be some kind of hash of the file name

        public uint TotalSize;

        public string BaseName;
        public string FileName;
        public string TypeName;

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.Build = (FileBuild)input.ReadValueU32(littleEndian);
            this.Unknown04 = input.ReadValueU16(littleEndian);
            this.Unknown06 = input.ReadValueU16(littleEndian);

            this.Type = input.ReadValueU32(littleEndian);

            this.Unknown0C = input.ReadValueU32(littleEndian);
            this.Type2 = input.ReadValueU32(littleEndian);
            this.Unknown14 = input.ReadValueU32(littleEndian);
            this.Unknown18 = input.ReadValueU32(littleEndian);

            this.TotalSize = input.ReadValueU32(littleEndian);

            this.BaseName = input.ReadStringZ();
            this.FileName = input.ReadStringZ();
            this.TypeName = input.ReadStringZ();
        }

        public string GetSaneFileName()
        {
            var name = this.FileName;
            var pos = name.LastIndexOf('\\');
            
            if (pos >= 0)
            {
                name = name.Substring(pos + 1);
            }

            if (name.Length > 50)
            {
                name = Path.ChangeExtension(name.Substring(0, 50), "." + this.TypeName);
            }
            else if (name.Length == 0)
            {
                name = Path.ChangeExtension("unknown", "." + this.TypeName);
            }

            return name;
        }
    }
}

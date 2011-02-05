using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Visceral.FileFormats.StreamSet
{
    public class FileInfo
    {
        public FileBuild Build;
        public ushort Alignment;
        public ushort Flags;
        
        public uint Type;

        public uint Unknown0C;
        public uint Type2;
        public uint Unknown14;
        public uint Unknown18; // seems to be some kind of hash of the file name

        public uint TotalSize;

        public string BaseName;
        public string FileName;
        public string TypeName;

        public void Serialize(Stream output, bool littleEndian)
        {
            output.WriteValueU32((uint)this.Build, littleEndian);
            output.WriteValueU16(this.Alignment, littleEndian);
            output.WriteValueU16(this.Flags, littleEndian);

            output.WriteValueU32(this.Type, littleEndian);

            output.WriteValueU32(this.Unknown0C, littleEndian);
            output.WriteValueU32(this.Type2, littleEndian);
            output.WriteValueU32(this.Unknown14, littleEndian);
            output.WriteValueU32(this.Unknown18, littleEndian);

            output.WriteValueU32(this.TotalSize, littleEndian);

            output.WriteStringZ(this.BaseName);
            output.WriteStringZ(this.FileName);
            output.WriteStringZ(this.TypeName);
        }

        public void Deserialize(Stream input, bool littleEndian)
        {
            this.Build = (FileBuild)input.ReadValueU32(littleEndian);
            this.Alignment = input.ReadValueU16(littleEndian);
            this.Flags = input.ReadValueU16(littleEndian);

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

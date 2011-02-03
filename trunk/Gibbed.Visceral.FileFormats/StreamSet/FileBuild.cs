using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.Visceral.FileFormats.StreamSet
{
    public enum FileBuild : uint
    {
        Default = 0x5393AC01, // "DEFAULT"
        Debug = 0x143C8453, // "Debug"
        Temporary = 0x473FCFB4, // "Temp"
    }
}

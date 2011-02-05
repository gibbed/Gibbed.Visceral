using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gibbed.Visceral.FileFormats.StreamSet
{
    public enum FileBuild : uint
    {
        Default = 0x5393AC01, // "default"
        Debug = 0x143C8453, // "debug"
        Temporary = 0x473FCFB4, // "temp"
        Geometry = 0x38B2B8D2, // "geometry"
        GeometryVolatile = 0x67A6002E, // "geometryvolatile"
        Volatile = 0x67456B5C, // "volatile"
        SoundBank = 0x7A10372B, // "soundbank"
        AnimationBank = 0x3D074740, // "animationbank"
    }
}

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

using System.Runtime.InteropServices;

namespace Gibbed.Visceral.FileFormats.SimGroup
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FileHeader
    {
        public uint Unknown00;
        public uint HeaderSize;
        public uint Unknown08Offset;
        public uint Unknown0COffset;
        public uint Unknown10Offset;
        public ushort Unknown14Count;
        public ushort Unknown16Count;
        public ushort Unknown18Count;
        public ushort Unknown1A;
        public uint Unknown1C;
        public uint Unknown20;
        public uint Type; // Hashes.SimGroup
        public uint Version; // 0x00010003
        public uint Unknown2C;
        public uint Unknown30;
        public uint Unknown34;
        public uint Unknown38;
    }
}

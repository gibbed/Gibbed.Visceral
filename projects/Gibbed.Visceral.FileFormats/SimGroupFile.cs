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
using System.IO;
using System.Runtime.InteropServices;
using Gibbed.IO;

namespace Gibbed.Visceral.FileFormats
{
    public class SimGroupFile
    {
        public void Deserialize(Stream input)
        {
            input.Seek(0, SeekOrigin.Begin);

            var header = input.ReadStructure<SimGroup.FileHeader>();

            if (header.HeaderSize < Marshal.SizeOf(header) ||
                header.HeaderSize > input.Length)
            {
                throw new FormatException("bad data size");
            }

            uint[] unknown08s = new uint[header.Unknown14Count];
            input.Seek(header.Unknown08Offset, SeekOrigin.Begin);
            for (ushort i = 0; i < header.Unknown14Count; i++)
            {
                unknown08s[i] = input.ReadValueU32();
            }

            input.Seek(header.Unknown0COffset, SeekOrigin.Begin);
            for (ushort i = 0; i < header.Unknown16Count; i++)
            {

            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Gibbed.Visceral.BigViewer
{
    internal class FileNameHashComparer : IComparer<uint>
    {
        private Dictionary<uint, string> FileNames;

        public FileNameHashComparer(Dictionary<uint, string> names)
        {
            this.FileNames = names;
        }

        public int Compare(uint x, uint y)
        {
            if (this.FileNames == null || this.FileNames.ContainsKey(x) == false)
            {
                if (this.FileNames == null || this.FileNames.ContainsKey(y) == false)
                {
                    if (x == y)
                    {
                        return 0;
                    }

                    return x < y ? -1 : 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (this.FileNames == null || this.FileNames.ContainsKey(y) == false)
                {
                    return 1;
                }
                else
                {
                    return String.Compare(this.FileNames[x], this.FileNames[y]);
                }
            }
        }
    }
}

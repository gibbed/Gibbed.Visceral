namespace Gibbed.Visceral.FileFormats.StreamSet
{
    public enum BlockType : uint
    {
        Options = 0x6F6C7333, // ols3
        Content = 0x53484F43, // SHOC
        Padding = 0x46494C4C, // FILL
    }
}

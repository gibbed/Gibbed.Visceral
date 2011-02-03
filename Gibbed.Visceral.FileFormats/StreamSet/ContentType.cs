namespace Gibbed.Visceral.FileFormats.StreamSet
{
    public enum ContentType : uint
    {
        Header = 0x53484452, // SHDR
        Data = 0x53444154, // SDAT
        CompressedData = 0x5270616B, // Rpak
    }
}

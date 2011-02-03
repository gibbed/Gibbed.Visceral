namespace Gibbed.Visceral.FileFormats
{
    public static class StringHelpers
    {
        public static uint HashName(this string input)
        {
            return input.ToLowerInvariant().HashFileName();
        }

        public static uint HashFileName(this string input)
        {
            //input = input.ToLowerInvariant();

            uint hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                hash = (hash * 65599) + (char)(input[i]);
            }

            return hash;
        }
    }
}

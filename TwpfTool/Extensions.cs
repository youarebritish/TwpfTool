using System.Collections.Generic;
using System.IO;

namespace TwpfTool
{
    public static class Extensions
    {
        public static string ReadCString(this BinaryReader reader)
        {
            var chars = new List<char>();
            var @char = reader.ReadChar();
            while (@char != '\0')
            {
                chars.Add(@char);
                @char = reader.ReadChar();
            }

            return new string(chars.ToArray());
        }
    }
}

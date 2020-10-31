using System;
using System.IO;

namespace TwpfTool
{
    internal sealed class Program
    {
        private static void Main(string[] args)
        {
            foreach (var path in args)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                var fileExtension = Path.GetExtension(path);
                if (fileExtension.Equals(".twpf", StringComparison.OrdinalIgnoreCase))
                {
                    var twpf = ReadFromBinary(path);
                    if (twpf != null)
                    {
                        //WriteToXml(vfx, Path.GetFileNameWithoutExtension(path) + ".vfx.xml");
                        continue;
                    }
                }
            }
        }

        private static WeatherParametersFile ReadFromBinary(string path)
        {
            var twpf = new WeatherParametersFile();
            using (var reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                if (twpf.Read(reader))
                {
                    return twpf;
                }
            }

            return null;
        }
    }
}

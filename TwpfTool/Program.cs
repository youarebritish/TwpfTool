using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TwpfTool
{
    internal static class Program
    {
        private const string DefinitionsPath = "definitions.json";

        private static void Main(string[] args)
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var definitions = ReadDefinitions(directory + "/" + DefinitionsPath);

            foreach (var path in args)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                var fileExtension = Path.GetExtension(path);
                if (fileExtension.Equals(".twpf", StringComparison.OrdinalIgnoreCase))
                {
                    var twpf = ReadFromBinary(path, definitions);
                    if (twpf != null)
                    {
                        var builder = new SpreadsheetBuilder(twpf, Path.GetFileNameWithoutExtension(path));
                        builder.Build();
                        continue;
                    }
                }
            }
        }

        private static WeatherParametersFile ReadFromBinary(string path, IDictionary<uint, StructDefinition> definitions)
        {
            var twpf = new WeatherParametersFile();
            using (var reader = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                if (twpf.Read(reader, definitions))
                {
                    return twpf;
                }
            }

            return null;
        }

        private static IDictionary<uint, StructDefinition> ReadDefinitions(string path)
        {
            var definitions = JsonConvert.DeserializeObject<StructDefinitions>(File.ReadAllText(path));
            return (from definition in definitions.structs
                    select definition).ToDictionary(val => val.id, val => val);
        }
    }
}

using System.Collections.Generic;

namespace TwpfTool
{
    public class StructDefinition
    {
        public uint id { get; set; }
        public string name { get; set; }
    }

    public class StructDefinitions
    {
        public List<StructDefinition> structs { get; set; }
    }
}

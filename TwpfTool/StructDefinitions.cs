using System.Collections.Generic;

namespace TwpfTool
{
    public class ParameterDefinition
    {
        public uint id { get; set; }
        public string name { get; set; }
    }

    public class StructDefinition
    {
        public uint id { get; set; }
        public string name { get; set; }
        public List<ParameterDefinition> parameters { get; set; } = new List<ParameterDefinition>();
    }

    public class StructDefinitions
    {
        public List<StructDefinition> structs { get; set; }
    }
}

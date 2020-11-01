using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace TwpfTool
{
    [DebuggerDisplay("{name}")]
    public sealed class GenericStruct
    {
        public uint StructType { get; private set; }
        public readonly IList<Parameter> Parameters = new List<Parameter>();

        private string name;

        public static GenericStruct Read(BinaryReader reader, IDictionary<uint, StructDefinition> definitions)
        {
            var instance = new GenericStruct();

            instance.StructType = reader.ReadUInt32();
            if (definitions.ContainsKey(instance.StructType))
            {
                instance.name = definitions[instance.StructType].name;
            }
            else
            {
                instance.name = $"Struct {instance.StructType}";
            }

            var parameterCount = instance.StructType & 0xFFFF;

            var paramOffsets = new uint[parameterCount];
            for (var i = 0; i < parameterCount; i++)
            {
                paramOffsets[i] = reader.ReadUInt32();
            }

            for (var i = 0; i < parameterCount; i++)
            {
                var param = Parameter.Read(reader);
                instance.Parameters.Add(param);
            }

            return instance;
        }

        public override string ToString()
        {
            return this.name;
        }
    }
}

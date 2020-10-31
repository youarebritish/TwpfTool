using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TwpfTool
{
    [DebuggerDisplay("Struct {StructType}")]
    public sealed class GenericStruct
    {
        public ushort StructType { get; private set; }
        public readonly IList<Parameter> Parameters = new List<Parameter>();

        public static GenericStruct Read(BinaryReader reader)
        {
            var instance = new GenericStruct();

            var parameterCount = reader.ReadUInt16();
            instance.StructType = reader.ReadUInt16();

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
    }
}

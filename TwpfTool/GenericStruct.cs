using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TwpfTool
{
    [DebuggerDisplay("Struct {StructType}")]
    public sealed class GenericStruct
    {
        public uint StructType { get; private set; }
        public readonly IList<Parameter> Parameters = new List<Parameter>();

        public static GenericStruct Read(BinaryReader reader)
        {
            var instance = new GenericStruct();

            instance.StructType = reader.ReadUInt32();
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
    }
}

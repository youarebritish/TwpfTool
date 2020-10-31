using System.Collections.Generic;
using System.IO;

namespace TwpfTool
{
    public sealed class TppGlobalVolumetricFog
    {
        public readonly IList<Parameter> Parameters = new List<Parameter>();

        public static TppGlobalVolumetricFog Read(BinaryReader reader, int parameterCount)
        {
            var instance = new TppGlobalVolumetricFog();
            for (var i = 0; i < parameterCount; i++)
            {
                var param = Parameter.Read(reader);
                instance.Parameters.Add(param);
            }

            return instance;
        }
    }
}

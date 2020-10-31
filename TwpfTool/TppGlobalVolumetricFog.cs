using System.Collections.Generic;
using System.IO;

namespace TwpfTool
{
    public sealed class TppGlobalVolumetricFog
    {
        public readonly IList<TppGlobalVolumetricFogParameter> Parameters = new List<TppGlobalVolumetricFogParameter>();

        public static TppGlobalVolumetricFog Read(BinaryReader reader, int parameterCount)
        {
            var instance = new TppGlobalVolumetricFog();
            for (var i = 0; i < parameterCount; i++)
            {
                var param = TppGlobalVolumetricFogParameter.Read(reader);
                instance.Parameters.Add(param);
            }

            return instance;
        }
    }
}

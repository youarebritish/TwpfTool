using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TwpfTool
{
    [DebuggerDisplay("TppGlobalVolumetricFogParameter Param {this.ParamId}")]
    public sealed class Parameter
    {
        public readonly IList<ParameterSetting> Settings = new List<ParameterSetting>();
        public ushort ParamId { get; private set; }

        public static Parameter Read(BinaryReader reader)
        {
            var instance = new Parameter();

            var settingCount = reader.ReadByte();
            var trackType = (WeatherParametersFile.TrackType)reader.ReadByte();
            instance.ParamId = reader.ReadUInt16();

            var settingOffsets = new uint[settingCount];
            for (var i = 0; i < settingCount; i++)
            {
                settingOffsets[i] = reader.ReadUInt32();
            }

            for (var i = 0; i < settingCount; i++)
            {
                var setting = ParameterSetting.Read(reader, trackType);
                instance.Settings.Add(setting);
            }

            return instance;
        }
    }
}

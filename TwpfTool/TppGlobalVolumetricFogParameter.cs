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
        public WeatherParametersFile.TrackType TrackType { get; private set; }

        public static Parameter Read(BinaryReader reader)
        {
            var instance = new Parameter();

            var settingCount = reader.ReadByte();
            instance.TrackType = (WeatherParametersFile.TrackType)reader.ReadByte();
            instance.ParamId = reader.ReadUInt16();

            var settingOffsets = new uint[settingCount];
            for (var i = 0; i < settingCount; i++)
            {
                settingOffsets[i] = reader.ReadUInt32();
            }

            for (var i = 0; i < settingCount; i++)
            {
                var setting = ParameterSetting.Read(reader, instance.TrackType);
                instance.Settings.Add(setting);
            }

            return instance;
        }
    }
}

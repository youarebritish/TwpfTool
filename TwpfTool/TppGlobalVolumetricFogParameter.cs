using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TwpfTool
{
    [DebuggerDisplay("Parameter {this.name}")]
    public sealed class Parameter
    {
        public readonly IList<ParameterSetting> Settings = new List<ParameterSetting>();
        public ushort ParamId { get; private set; }
        public WeatherParametersFile.TrackType TrackType { get; private set; }
        private string name;

        public static Parameter Read(BinaryReader reader, StructDefinition definition)
        {
            var instance = new Parameter();

            var settingCount = reader.ReadByte();
            instance.TrackType = (WeatherParametersFile.TrackType)reader.ReadByte();
            instance.ParamId = reader.ReadUInt16();

            if (definition != null)
            {
                var paramDefinition = (from param in definition.parameters
                                       where param.id == instance.ParamId
                                       select param).ToList();

                if (paramDefinition.Count > 0)
                {
                    instance.name = paramDefinition[0].name;
                }
                else
                {
                    instance.name = $"Param {instance.ParamId}";
                }
            }
            else
            {
                instance.name = $"Param {instance.ParamId}";
            }

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

        public override string ToString()
        {
            return this.name;
        }
    }
}

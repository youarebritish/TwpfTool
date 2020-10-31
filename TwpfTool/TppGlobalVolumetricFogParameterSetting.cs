using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TwpfTool
{
    [DebuggerDisplay("{Tag}")]
    public sealed class ParameterSetting
    {
        public string Tag { get; private set; }
        public readonly IList<ITrack> Tracks = new List<ITrack>();

        public static ParameterSetting Read(BinaryReader reader, WeatherParametersFile.TrackType trackType)
        {
            var instance = new ParameterSetting();

            var tagOffset = reader.ReadUInt32();
            var position = reader.BaseStream.Position;

            reader.BaseStream.Seek(tagOffset, SeekOrigin.Begin);
            instance.Tag = reader.ReadCString();
            reader.BaseStream.Seek(position, SeekOrigin.Begin);

            var numTracks = reader.ReadByte();
            var tagIndex = reader.ReadUInt16();
            reader.ReadByte();

            var trackOffsets = new uint[numTracks];
            for (var i = 0; i < numTracks; i++)
            {
                trackOffsets[i] = reader.ReadUInt32();
            }

            for (var i = 0; i < numTracks; i++)
            {
                instance.ReadTrack(reader, trackType);
            }

            return instance;
        }

        private void ReadTrack(BinaryReader reader, WeatherParametersFile.TrackType type)
        {
            ITrack track;
            switch (type)
            {
                case WeatherParametersFile.TrackType.Strength:
                    track = Track<StrengthKeyframe>.Read(reader);
                    break;
                case WeatherParametersFile.TrackType.Rgb:
                    track = Track<ColorKeyframe>.Read(reader);
                    break;
                case WeatherParametersFile.TrackType.Lut:
                    track = Track<LutKeyframe>.Read(reader);
                    break;
                default:
                    throw new NotImplementedException($"Unknown track type: {type}");
            }

            this.Tracks.Add(track);
        }
    }
}

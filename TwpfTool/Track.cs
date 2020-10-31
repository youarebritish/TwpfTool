using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TwpfTool
{
    [DebuggerDisplay("Weather {WeatherId}")]
    public sealed class Track<TKeyframe> : ITrack where TKeyframe : Keyframe, new()
    {
        public ushort WeatherId { get; private set; }
        public readonly IList<TKeyframe> Keyframes = new List<TKeyframe>();

        public static Track<TKeyframe> Read(BinaryReader reader)
        {
            var instance = new Track<TKeyframe>();
            instance.WeatherId = reader.ReadUInt16();

            var numFrames = reader.ReadUInt16();
            var frameOffsets = new uint[numFrames];
            for (var i = 0; i < numFrames; i++)
            {
                frameOffsets[i] = reader.ReadUInt32();
            }

            for (var i = 0; i < numFrames; i++)
            {
                var frame = new TKeyframe();
                frame.Read(reader);

                instance.Keyframes.Add(frame);
            }

            return instance;
        }

        public IList<Keyframe> GetKeyframes()
        {
            return (from keyframe in this.Keyframes
                    select keyframe as Keyframe).ToList();
        }
    }
}

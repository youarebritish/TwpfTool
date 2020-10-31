using System.Collections.Generic;
using System.IO;

namespace TwpfTool
{
    public interface ITrack
    {
        ushort WeatherId { get; }
        IList<Keyframe> GetKeyframes();
    }
}

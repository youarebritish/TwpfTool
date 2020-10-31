using System.Diagnostics;
using System.IO;

namespace TwpfTool
{
    [DebuggerDisplay("{Time}: {Value}")]
    public sealed class LutKeyframe : Keyframe
    {
        public ulong Value { get; private set; }
        protected override void ReadValue(BinaryReader reader)
        {
            this.Value = reader.ReadUInt64();
        }
    }
}

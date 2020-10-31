using System.IO;

namespace TwpfTool
{
    public abstract class Keyframe
    {
        public uint Time { get; private set; }

        public void Read(BinaryReader reader)
        {
            this.Time = reader.ReadUInt32();
            this.ReadValue(reader);
        }

        protected abstract void ReadValue(BinaryReader reader);
    }
}

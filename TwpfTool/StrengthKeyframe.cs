using NanoXLSX;
using NanoXLSX.Styles;
using System.Diagnostics;
using System.IO;

namespace TwpfTool
{
    [DebuggerDisplay("{Time}: {Value}")]
    public sealed class StrengthKeyframe : Keyframe
    {
        public float Value { get; private set; }
        protected override void ReadValue(BinaryReader reader)
        {
            this.Value = reader.ReadSingle();
        }

        public override void WriteValue(Workbook workbook, Style style)
        {
            workbook.WS.Value(this.Value, style);
        }
    }
}

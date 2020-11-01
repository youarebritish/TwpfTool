using NanoXLSX;
using NanoXLSX.Styles;
using System.Diagnostics;
using System.IO;

namespace TwpfTool
{
    [DebuggerDisplay("({Red}, {Green}, {Blue})")]
    public struct Color
    {
        public readonly float Red;
        public readonly float Green;
        public readonly float Blue;

        public Color(float red, float green, float blue)
        {
            this.Red = red;
            this.Green = green;
            this.Blue = blue;
        }
    }

    [DebuggerDisplay("{Time}: {Value}")]
    public sealed class ColorKeyframe : Keyframe
    {
        public Color Value { get; private set; }
        protected override void ReadValue(BinaryReader reader)
        {
            var red = reader.ReadSingle();
            var green = reader.ReadSingle();
            var blue = reader.ReadSingle();

            this.Value = new Color(red, green, blue);
        }

        public override void WriteValue(Workbook workbook, Style style)
        {
            workbook.WS.Value($"{this.Value.Red}, {this.Value.Green}, {this.Value.Blue}", style);
        }
    }
}

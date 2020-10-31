using NanoXLSX;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TwpfTool
{
    public class SpreadsheetBuilder
    {
        private readonly WeatherParametersFile twpf;
        private readonly Workbook workbook;
        public SpreadsheetBuilder (WeatherParametersFile twpf, string name)
        {
            this.twpf = twpf;
            this.workbook = new Workbook($"{name}.xlsx", "Sheet1");
        }

        public void Build(string name)
        {
            /*var tags = GetTags();

            foreach (var tag in tags)
            {
                this.RenderTagRow(tag);
            }*/

            workbook.Save();
        }

        private void RenderTagRow(string tag)
        {
            this.workbook.WS.Value("Tag");
            this.workbook.WS.Value(tag);
        }
    }
}

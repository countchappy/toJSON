using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using toJSON.Classes.Output.Templates;

namespace toJSON.Classes.Output
{
    internal class OutputEngine
    {
        public OutputTemplate Template { get; set; }

        public OutputEngine(OutputTemplateType templateType, FileInfo templateFile = null)
        {
            if (templateType == OutputTemplateType.JSON) Template = new JSONTemplate(templateFile);
            Template.Initialize();
        }

        public void AddRow(string[] row, bool isHeader = false)
        {
            for (var c = 0; c < row.Length; c++)
            {
                if (!isHeader) Template.MapCell(c, row[c]);
                else
                {
                    Template.Headers = row;
                    break;
                }
            }
        }

        public string GenerateOutput()
        {
            Template.Cleanup();
            return Template.GetOutput();
        }
    }
}

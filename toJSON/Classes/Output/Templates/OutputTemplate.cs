using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.Output.Templates
{
    abstract class OutputTemplate
    {
        public abstract string[] Headers { get; set; }
        public abstract void Initialize();
        public abstract void Cleanup();
        public abstract string GetOutput();
        public abstract void MapCell(int Column, string Value);
    }

    public enum OutputTemplateType
    {
        JSON,
        Invalid
    }
}

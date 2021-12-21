using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using toJSON.Classes.InputEngines;
using toJSON.Classes.Output;
using toJSON.Classes.Output.Templates;

namespace toJSON.Classes
{
    class FileInputManager
    {
        FileInfo Input;
        FileInfo Output;

        OutputTemplateType OutputTemplateType { get; set; }

        public FileInfo OutputTemplate { get; set; }

        public FileInputManager(OutputTemplateType outputTemplateType, FileInfo input, FileInfo output)
        {
            OutputTemplateType = outputTemplateType;
            Input = input;
            Output = output;
        }

        public void ProcessFile(Engine engine)
        {
            var dataSet = engine.ProcessData(Input);
            engine.Shutdown();
            engine = null;


            var outputEngine = OutputTemplate == null ? new OutputEngine(OutputTemplateType) : new OutputEngine(OutputTemplateType, OutputTemplate);
            for (int rn = 0; rn < dataSet.Count; rn++)
            {
                outputEngine.AddRow(dataSet[rn], rn==0);
            }

            StreamWriter tmp = new StreamWriter(Output.FullName);
            tmp.Write(outputEngine.GenerateOutput());
            tmp.Flush();
            tmp.Close();
        }
    }
}

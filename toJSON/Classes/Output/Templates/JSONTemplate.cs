using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using toJSON.Classes.Output.Templates.JSON;
using Newtonsoft.Json;

namespace toJSON.Classes.Output.Templates
{
    class JSONTemplate : OutputTemplate
    {
        public JObject Template { get; set; }
        public override string[] Headers { get; set; }

        private Node workingObject { get; set; }

        public List<JToken> Results { get; set; } = new List<JToken>();

        public JSONTemplate(JObject Template = null)
        {
            if (Template == null)
                this.Template = JObject.Parse("{\"${header-${I}}\": \"${column-${I}}\"}");
            else
                this.Template = Template;
        }

        public JSONTemplate(FileInfo templateFile)
        {
            if (templateFile == null) Template = JObject.Parse("{\"${header-${I}}\": \"${column-${I}}\"}");
            else
            {
                var rawTemplate = File.ReadAllText(templateFile.FullName);
                Template = JObject.Parse(rawTemplate);
            }
        }

        private void WorkingObjectComplete(object sender, BubbleFullEventArgs args)
        {
            Cleanup();
            MapCell(args.Column, args.Value);
        }

        public override void MapCell(int Column, string Value)
        {
            workingObject.MapCell(Column, Value);
        }

        public override void Initialize()
        {
            workingObject = new ObjectNode(Template);
            workingObject.BubbleFull += WorkingObjectComplete;
        }

        public override void Cleanup()
        {
            workingObject.IsFill = false;

            if (Headers != null)
                for (int i = 0; i < Headers.Length; i++)
                    workingObject.MapHeader(i, Headers[i]);

            Results.Add(workingObject.GetJToken());

            Initialize();
        }

        public override string GetOutput()
        {
            return JsonConvert.SerializeObject(Results, Formatting.Indented);
        }
    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.Output.Templates.JSON
{
    internal class FillArrayNode : Node
    {
        public string Key { get; private set; }
        public List<string> Children { get; private set; } = new List<string>();
        public Type ValueType = typeof(string);

        private List<int> filledColumns = new List<int>();


        private string boolCompare = null;

        public FillArrayNode(JProperty token, Node parent) : base(token, parent)
        {
            var jArray = token.Value as JArray;
            string child = (string)jArray.Children().FirstOrDefault();
            if (child != null)
            {
                var cMatches = NodeRegex.match_token.Match(child);
                if (cMatches.Groups.Count > 0)
                {
                    ValueType = NodeRegex.MapCast(cMatches.Groups[1].ToString().ToLower());
                    if (ValueType == typeof(bool)) boolCompare = cMatches.Groups[3].ToString();
                }
                IsFill = true;
                Key = token.Name;
            }
        }

        public override bool IsFill { get; set; }

        public override JToken GetJToken()
        {
            var jArray = new JArray();
            foreach(var item in Children) jArray.Add(item);
            return new JProperty(Key, jArray);
        }

        public override bool MapCell(int Column, string Value)
        {
            if (!IsFill) return false;
            if (filledColumns.Contains(Column))
            {
                IsFill = false;
                Full(this, new BubbleFullEventArgs(Column, Value));
                return true;
            }
            else
            {
                Children.Add(Value);
                return true;
            }
        }

        public override void MapHeader(int Column, string Header)
        {
            return;
        }

        public override void ClearFill()
        {
            foreach(var item in Children)
            {
                
            }
        }
    }
}

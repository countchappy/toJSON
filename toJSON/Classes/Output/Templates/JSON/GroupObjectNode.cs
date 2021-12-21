using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.Output.Templates.JSON
{
    internal class GroupObjectNode : Node
    {
        public ObjectNode Child { get; private set; }

        public int ColumnIndex { get; private set; }
        public string Key { get; private set; }

        private bool keySet = false;

        public override bool IsFill
        {
            get
            {
                return Child.IsFill;
            }
            set
            {
                Child.IsFill = value;
            }
        }

        public GroupObjectNode(JProperty token, Node parent) : base(token, parent)
        {
            var keyMatches = NodeRegex.match_token.Match(token.Name);
            if (keyMatches.Groups.Count == 8)
            {
                Key = keyMatches.Groups[7].ToString();
                if (NodeRegex.match_digit.IsMatch(Key))
                {
                    var ciString = NodeRegex.match_digit.Match(Key).ToString();
                    int val;
                    if (int.TryParse(ciString, out val)) ColumnIndex = val;
                    Child = new ObjectNode(token.Value, this);
                    Child.BubbleFull += ChildrenFull;
                }
            }
        }

        private void ChildrenFull(object sender, BubbleFullEventArgs args)
        {
            IsFill = false;
            Full(this, args);
        }

        public override JToken GetJToken()
        {
            return new JProperty(Key, Child.GetJToken());
        }

        public override bool MapCell(int Column, string Value)
        {
            if (Column == ColumnIndex)
            {
                if (keySet == false)
                {
                    Key = Value;
                    keySet = true;
                }
                else if (!Key.Equals(Value))
                {
                    Full(this, new BubbleFullEventArgs(Column, Value));
                }
                return true;
            }
            else if (Child.MapCell(Column, Value)) return true;
            return false;
        }

        public override void MapHeader(int Column, string Header)
        {
            Child.MapHeader(Column, Header);
        }
        
        // Might need to think about how this will delete
        public override void ClearFill()
        {
            Child.ClearFill();
        }
    }
}

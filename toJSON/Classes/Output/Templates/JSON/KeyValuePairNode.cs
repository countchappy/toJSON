using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.Output.Templates.JSON
{
    internal class KeyValuePairNode : Node
    {
        public override bool IsFill { get; set; }

        public int HeaderIndex { get; private set; } = -1;
        public int ColumnIndex { get; private set; } = -1;

        public string Key { get; private set; }
        public string Value { get; private set; }

        private bool valueSet { get; set; }
        private string boolCompare { get; set; }

        public ValueType ValueType { get; set; } = ValueType.String;

        public KeyValuePairNode(JProperty token, Node parent) : base(token, parent)
        {
            Key = token.Name;
            Value = token.Value.ToString();
            var keyMatches = NodeRegex.match_token.Match(Key);
            var valueMatches = NodeRegex.match_token.Match(Value);

            if (keyMatches.Groups.Count == 8)
            {
                if (NodeRegex.match_fill.IsMatch(keyMatches.Groups[6].ToString())) IsFill = true;
                if (NodeRegex.match_digit.IsMatch(keyMatches.Groups[6].ToString()))
                {
                    int headerIndex;
                    if (int.TryParse(keyMatches.Groups[6].ToString(), out headerIndex)) HeaderIndex = headerIndex;
                }
            }

            if (valueMatches.Groups.Count == 8)
            {
                if (NodeRegex.match_fill.IsMatch(valueMatches.Groups[6].ToString())) IsFill = true;
                if (NodeRegex.match_digit.IsMatch(valueMatches.Groups[6].ToString()))
                {
                    int columnIndex;
                    if (int.TryParse(valueMatches.Groups[6].ToString(), out columnIndex)) ColumnIndex = columnIndex;
                }

                var cast = valueMatches.Groups[1].ToString().ToLower();
                if (!string.IsNullOrEmpty(cast))
                {
                    ValueType = NodeRegex.MapCast(valueMatches.Groups[1].ToString().ToLower());
                    if (ValueType == ValueType.Boolean) boolCompare = valueMatches.Groups[3].ToString();
                }
            }
        }

        public override JToken GetJToken()
        {
            return new JProperty(Key, NodeRegex.CastValue(ValueType, Value, boolCompare));
        }

        public override void MapHeader(int Column, string Header)
        {
            if ((HeaderIndex == Column || IsFill) && NodeRegex.match_token.IsMatch(Key))
                Key = NodeRegex.match_token.Replace(Key, Header);
        }

        public override bool MapCell(int Column, string Value)
        {
            if (IsFill)
            {
                ColumnIndex = Column;
                this.Value = Value;
                valueSet = true;

                var keyMatches = NodeRegex.match_token.Match(Key);
                if (keyMatches.Groups.Count > 0)
                {
                    if (NodeRegex.match_fill.IsMatch(keyMatches.Groups[6].ToString()))
                    {
                        HeaderIndex = ColumnIndex;
                        Key = NodeRegex.match_fill.Replace(Key, ColumnIndex.ToString());
                    }
                }

                Full(this, null);

                return true;
            }
            else
            {
                if (ColumnIndex != Column) return false;
                if (valueSet)
                {
                    if (!this.Value.Equals(Value))
                    {
                        Full(this, new BubbleFullEventArgs(Column, Value));
                    }
                }
                else
                {
                    this.Value = Value;
                    valueSet = true;
                }
                return true;
            }
        }

        /*
        public override void ClearFill()
        {
            return;
        }
        */
    }
}

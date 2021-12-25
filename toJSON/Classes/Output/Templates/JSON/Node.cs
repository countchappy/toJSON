using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace toJSON.Classes.Output.Templates.JSON
{
    class NodeRegex
    {
        public static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static Regex match_token = new Regex(@"(?:\${(int|bool|float|datetime|epochmillis)?(\[(.*)\])?\(?((\w*)[-=](\d+|\[(.*)\]|\${I}))\)?})");
        public static Regex match_fill = new Regex(@"(?:\$\{I\})");
        public static Regex match_digit = new Regex(@"(?:\d+)");

        public static ValueType MapCast(string castKey)
        {
            switch (castKey)
            {
                case "int": return ValueType.Integer;
                case "float": return ValueType.Float;
                case "datetime": return ValueType.DateTime;
                case "bool": return ValueType.Boolean;
                case "epochmillis": return ValueType.EpochMillis;
                default: return ValueType.String;
            }
        }
        
        public static object CastValue(ValueType valueType, string value, string booleanComparitor)
        {
            object returnValue = null;

            switch (valueType)
            {
                case ValueType.Integer:
                    int integerValue;
                    if (int.TryParse(value, out integerValue))
                        returnValue = integerValue;
                    break;
                case ValueType.Float:
                    float floatValue;
                    if (float.TryParse(value, out floatValue))
                        returnValue = floatValue;
                    break;
                case ValueType.Boolean:
                    returnValue = value.Equals(booleanComparitor);
                    break;
                case ValueType.DateTime:
                    DateTime dateTimeValue;
                    if (DateTime.TryParse(value, out dateTimeValue))
                        returnValue = dateTimeValue;
                    break;
                case ValueType.EpochMillis:
                    DateTime epochMillisValue;
                    if (DateTime.TryParse(value, out epochMillisValue))
                        returnValue = epochMillisValue.ToUniversalTime().Subtract(NodeRegex.UnixEpoch).TotalMilliseconds;
                    break;
                default:
                    if (!string.IsNullOrEmpty(value))
                        returnValue = value;
                    break;
            }

            return returnValue;
        }
    }

    enum ValueType
    {
        Integer,
        Float,
        DateTime,
        Boolean,
        EpochMillis,
        String
    }

    abstract class Node
    {
        public JToken Token { get; set; }
        public Node Parent { get; set; }

        public abstract bool IsFill { get; set; }

        public abstract void MapHeader(int Column, string Header);

        public abstract bool MapCell(int Column, string Value);

        public abstract JToken GetJToken();

        public override string ToString()
        {
            return GetJToken().ToString();
        }

        public Node(JToken token, Node parent)
        {
            Token = token;
            Parent = parent;
        }

        protected virtual void Full(object sender, BubbleFullEventArgs args)
        {
            if (sender == null) sender = this;
            EventHandler<BubbleFullEventArgs> handler = BubbleFull;
            if (handler != null) handler(sender, args);
        }

        public event EventHandler<BubbleFullEventArgs> BubbleFull;
    }

    public class BubbleFullEventArgs : EventArgs
    {
        public int Column { get; set; }
        public string Value { get; set; }

        public BubbleFullEventArgs(int Column, string Value) : base()
        {
            this.Column = Column;
            this.Value = Value;
        }
    }
}

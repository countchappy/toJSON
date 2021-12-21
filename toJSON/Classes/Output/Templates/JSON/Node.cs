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
        public static Regex match_token = new Regex(@"(?:\${(int|bool|float|datetime)?(\[(.*)\])?\(?((\w*)[-=](\d+|\[(.*)\]|\${I}))\)?})");
        public static Regex match_fill = new Regex(@"(?:\$\{I\})");
        public static Regex match_digit = new Regex(@"(?:\d+)");

        public static Type MapCast(string castKey)
        {
            switch (castKey)
            {
                case "int": return typeof(int);
                case "float": return typeof(float);
                case "datetime": return typeof(DateTime);
                case "bool": return typeof(bool);
                default: return typeof(string);
            }
        }
    }

    abstract class Node
    {
        public JToken Token { get; set; }
        public Node Parent { get; set; }

        public abstract bool IsFill { get; set; }

        public abstract void MapHeader(int Column, string Header);

        public abstract bool MapCell(int Column, string Value);

        public abstract JToken GetJToken();

        public abstract void ClearFill();

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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.Output.Templates.JSON
{
    internal class ObjectNode : Node
    {
        public List<Node> Children { get; private set; } = new List<Node>();

        public override bool IsFill { get { return false; }
            set
            {
                if (!value)
                {
                    Children.ToList().ForEach(child =>
                    {
                        child.IsFill = false;
                    });
                }
            }
        }

        public ObjectNode(JToken token, Node parent = null) : base(token, parent)
        {
            JObject curObject = (JObject)token;
            foreach (var jPropChild in curObject.Children<JProperty>().ToList())
                BirthNewChild(jPropChild);
        }

        private void BirthNewChild(JProperty token)
        {
            Node child = null;
            switch (token.Value.Type)
            {
                case JTokenType.String:
                    child = new KeyValuePairNode(token, this);
                    break;
                case JTokenType.Object:
                    child = new GroupObjectNode(token, this);
                    break;
                case JTokenType.Array:
                    var jArray = (JArray)token.Value;
                    var firstChild = jArray.Children().FirstOrDefault();
                    switch(firstChild?.Type)
                    {
                        case JTokenType.Object:
                            child = new GroupArrayNode(token, this);
                            break;
                        case JTokenType.String:
                            child = new FillArrayNode(token, this);
                            break;
                    }
                    break;
            }
            if (child != null)
            {
                child.BubbleFull += ChildrenFull;
                Children.Add(child);
            }
        }

        public override JToken GetJToken()
        {
            var obj = new JObject();
            foreach (var child in Children)
            {
                var token = child.GetJToken();
                if (token != null) obj.Add(token);
            }

            return obj;
        }

        public override void MapHeader(int Column, string Header)
        {
            foreach (var child in Children.ToList())
                child.MapHeader(Column, Header);
        }

        public override bool MapCell(int Column, string Value)
        {
            foreach (var child in Children.ToList())
                if (child.MapCell(Column, Value))
                    return true;

            return false;
        }

        public void ChildrenFull(object sender, BubbleFullEventArgs args)
        {
            if (sender is KeyValuePairNode && ((KeyValuePairNode)sender).IsFill)
            {
                ClearFill();
                KeyValuePairNode pair = (KeyValuePairNode)sender;
                BirthNewChild((JProperty)pair.Token);
            }
            else if (sender is GroupObjectNode)
            {
                ClearFill();
                GroupObjectNode groupObject = (GroupObjectNode)sender;
                BirthNewChild((JProperty)groupObject.Token);
                if (args != null)
                    MapCell(args.Column, args.Value);
            }
            else
            {
                Full(sender, args);
            }
        }

        public override void ClearFill()
        {
            foreach(var child in Children.ToList())
            {
                if (child is KeyValuePairNode && child.IsFill)
                {
                    Children.Remove(child);
                } else
                {
                    child.ClearFill();
                }
            }
        }
    }
}

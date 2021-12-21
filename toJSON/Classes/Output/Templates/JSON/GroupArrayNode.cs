using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace toJSON.Classes.Output.Templates.JSON
{
    internal class GroupArrayNode : Node
    {
        public List<Node> Children { get; private set; } = new List<Node>();
        public JToken ChildTemplate { get; private set; }

        public string Key { get; private set; }

        public override bool IsFill
        {
            get
            {
                if (Children.Any(c => c.IsFill)) return true;
                return false;
            }
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

        public int HeaderIndex { get; private set; }

        public GroupArrayNode(JProperty token, Node parent) : base(token, parent)
        {
            var val = (JArray)token.Value;

            var keyMatches = NodeRegex.match_token.Match(token.Name);
            if (keyMatches.Groups.Count > 0)
            {
                Key = keyMatches.Groups[7].ToString();
                ChildTemplate = val[0];

                BirthChild();
            }
        }

        public override JToken GetJToken()
        {
            var arr = new JArray();
            foreach (var child in Children)
            {
                var token = child.GetJToken();
                if (token != null) arr.Add(token);
            }

            return new JProperty(Key, arr);
        }

        public override bool MapCell(int Column, string Value)
        {
            foreach (var child in Children.ToList())
                if (child.MapCell(Column, Value))
                    return true;

            return false;
        }

        public override void MapHeader(int Column, string Header)
        {
            if (HeaderIndex == Column)
                if (NodeRegex.match_token.IsMatch(Key))
                    Key = NodeRegex.match_token.Replace(Key, Header);

            foreach (var child in Children.ToList())
                child.MapHeader(Column, Header);
        }

        private void ChildrenFull(object sender, BubbleFullEventArgs args)
        {
            IsFill = false;
            var c = BirthChild();
            if (args != null) c.MapCell(args.Column, args.Value);
        }

        private ObjectNode BirthChild()
        {
            var child = new ObjectNode(ChildTemplate, this);
            child.BubbleFull += ChildrenFull;
            Children.Add(child);
            return child;
        }

        public override void ClearFill()
        {
            foreach (var child in Children)
            {
                if (child.IsFill) Children.Remove(child);
            }
        }
    }
}

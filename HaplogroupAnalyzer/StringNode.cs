using System.Collections.Generic;

namespace HaplogroupAnalyzer
{
    public class StringNode
    {
        public string Value { get; set; }

        public List<StringNode> Children { get; set; }

        public StringNode()
        {
            Children = new List<StringNode>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class TemplateToken
    {
        public TemplateTokenType Type { get; set; }

        public string Value { get; set; }

        public TemplateToken()
        {
        }

        public TemplateToken(TemplateTokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}

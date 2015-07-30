using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class StringTemplate
    {
        public TemplateLexer Lexer { get; set; }

        public string Text { get; set; }

        public bool IsStrict { get; set; }

        public StringTemplate()
        {
            Lexer = new TemplateLexer();
            IsStrict = true;
        }

        public StringTemplate(string text)
            : this()
        {
            Text = text;
        }

        public string Populate(Dictionary<string, string> values)
        {
            var sb = new StringBuilder();
            var tokens = Lexer.Tokenize(Text);

            for (int i = 0; i < tokens.Count; i++)
            {
                var t = tokens[i];
                switch (t.Type)
                {
                    case TemplateTokenType.String:
                        sb.Append(t.Value);
                        break;

                    case TemplateTokenType.Token:

                        string value;
                        if (values.TryGetValue(t.Value, out value))
                        {
                            sb.Append(value);
                        }
                        else if (IsStrict)
                        {
                            throw new InvalidOperationException();
                        }

                        break;
                }
            }

            return sb.ToString();
        }

        public string PopulateObj(object obj)
        {
            return Populate(obj
                .GetType()
                .GetProperties()
                .ToDictionary(x => x.Name, x => x.GetValue(obj).ToString()));
        }
    }
}

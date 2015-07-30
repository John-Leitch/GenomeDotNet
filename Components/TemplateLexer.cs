using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class TemplateLexer
    {
        public char TokenStart { get; set; }

        public char TokenEnd { get; set; }

        public TemplateLexer(char tokenStart, char tokenEnd)
        {
            TokenStart = tokenStart;
            TokenEnd = tokenEnd;
        }

        public TemplateLexer(char delimiter)
            : this(delimiter, delimiter)
        {
        }

        public TemplateLexer()
            : this('{', '}')
        {

        }

        public List<TemplateToken> Tokenize(string template)
        {
            var state = TemplateLexerState.InText;
            var currentToken = new StringBuilder();
            var tokens = new List<TemplateToken>();

            for (int i = 0; i < template.Length; i++)
            {
                var c = template[i];

                switch (state)
                {
                    case TemplateLexerState.InText:

                        if (c == TokenStart) state = TemplateLexerState.InTokenStart;
                        else if (c == TokenEnd) state = TemplateLexerState.InTokenEndEscapeSequence;
                        else currentToken.Append(c);

                        break;

                    case TemplateLexerState.InTokenStart:

                        if (c == TokenStart)
                        {
                            currentToken.Append(TokenStart);
                            state = TemplateLexerState.InText;
                        }
                        else
                        {
                            tokens.Add(new TemplateToken(TemplateTokenType.String, currentToken.ToString()));
                            currentToken.Clear();
                            currentToken.Append(c);
                            state = TemplateLexerState.InToken;
                        }

                        break;

                    case TemplateLexerState.InToken:

                        if (c == TokenEnd)
                        {
                            tokens.Add(new TemplateToken(TemplateTokenType.Token, currentToken.ToString()));
                            currentToken.Clear();
                            state = TemplateLexerState.InText;
                        }
                        else
                        {
                            currentToken.Append(c);
                        }

                        break;

                    case TemplateLexerState.InTokenEndEscapeSequence:

                        if (c == TokenEnd)
                        {
                            currentToken.Append(c);
                            state = TemplateLexerState.InText;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }

                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            switch (state)
            {
                case TemplateLexerState.InText:
                    tokens.Add(new TemplateToken(TemplateTokenType.String, currentToken.ToString()));
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return tokens;
        }
    }
}

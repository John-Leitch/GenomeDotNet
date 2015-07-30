using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Components.Net.ObjectService
{
    public class BatchScriptCommand
    {
        public string Text { get; set; }

        public string RawResult { get; set; }

        public string Result { get; set; }

        public BatchScriptCommand(string text, string rawResult)
        {
            Text = text;
            RawResult = rawResult;
            Result = CleanRawResult();
        }

        public string CleanRawResult()
        {
            if (string.IsNullOrEmpty(RawResult))
            {
                return RawResult;
            }

            var lines = RawResult.SplitLines();

            if (lines.First() == Text)
            {
                lines = lines.Skip(1).ToArray();
            }

            if (Regex.IsMatch(lines.Last(), @"[a-zA-Z]:\\.*?\>$"))
            {
                lines = lines.Take(lines.Length - 1).ToArray();
            }

            return lines.Join("\r\n");
        }

        public override string ToString()
        {
            return string.Format("{{ {0}, {1} }}", Text, Result);
        }
    }
}

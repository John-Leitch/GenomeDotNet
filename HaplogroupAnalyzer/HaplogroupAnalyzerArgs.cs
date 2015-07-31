using Components;
using System.IO;

namespace HaplogroupAnalyzer
{
    public class HaplogroupAnalyzerArgs
    {
        [CliArg(
            "-i",
            ValueName = "raw data file",
            Description = "A 23andme raw data file.",
            IsInputFile = true,
            IsRequired = true)]
        public FileInfo DataFile { get; set; }
    }
}

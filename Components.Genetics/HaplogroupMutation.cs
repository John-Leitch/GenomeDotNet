using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Genetics
{
    [Serializable]
    public class HaplogroupMutation
    {
        public string Snp { get; set; }

        public string Haplogroup { get; set; }

        public string Rsid { get; set; }

        public int Position { get; set; }

        public char OldNucleotide { get; set; }

        public char NewNucleotide { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0} {1} {2} {3}->{4}",
                Haplogroup,
                Snp,
                Position,
                OldNucleotide,
                NewNucleotide);
        }
    }
}

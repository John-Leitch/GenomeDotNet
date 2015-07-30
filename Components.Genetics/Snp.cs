using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Genetics
{
    public class Snp
    {
        public string Rsid { get; set; }

        public int Position { get; set; }

        public string Genotype { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Rsid, Position, Genotype);
        }
    }
}

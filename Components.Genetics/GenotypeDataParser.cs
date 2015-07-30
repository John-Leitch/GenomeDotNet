using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Genetics
{
    public static class GenotypeDataParser
    {
        public static List<Snp> Parse(string filename)
        {
            var snps = new List<Snp>();

            using (var reader = new StreamReader(filename))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrEmpty(line) || line[0] == '#')
                    {
                        continue;
                    }

                    var parts = line.Split('\t');

                    if (parts[1] != "Y")
                    {
                        continue;
                    }

                    snps.Add(new Snp()
                    {
                        Rsid = parts[0],
                        Position = int.Parse(parts[2]),
                        Genotype = parts[3],
                    });
                }
            }

            return snps;
        }

        public static Dictionary<int, Snp> GetSnpTable(string filename)
        {
            Console.WriteLine("Loading SNPs");
            var snps = Parse(filename);
            Console.WriteLine("Creating tables");
            //var rsidTable = snps.ToDictionary(x => x.Rsid);
            return snps.GroupBy(x => x.Position).ToDictionary(x => x.Key, x => x.First());
        }
    }
}

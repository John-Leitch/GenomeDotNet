using Components;
using Components.ConsolePlus;
using Components.Genetics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HaplogroupAnalyzer
{
    public class HaplogroupAnalyzerApplication : CliApplication<HaplogroupAnalyzerArgs>
    {
        private static Haplogroup FindHaplogroup(string name, Haplogroup group)
        {
            if (group.Name == name)
            {
                return group;
            }
            else if (group.Children.Any())
            {
                return group.Children
                    .Select(x => FindHaplogroup(name, x))
                    .FirstOrDefault(x => x != null);
            }
            else
            {
                return null;
            }
        }

        private Haplogroup LoadYDnaTree()
        {
            try
            {
                var ydnaJson = PathHelper.GetExecutingPath("ydnatree.json");
                var root = JsonSerializer.DeserializeFile<Haplogroup>(ydnaJson);

                root.Visit(x =>
                {
                    foreach (var c in x.Children)
                    {
                        c.Parent = x;
                    }
                });

                return root;
            }
            catch (Exception e)
            {
                WriteFatalError(0x1001, "Could not load Y-DNA haplogroup tree: {0}", e.Message);

                return null;
            }
        }

        public override void Main(HaplogroupAnalyzerArgs args)
        {
            WriteInfoMessage(
                "Loading data file ~Cyan~{0}~R~",
                Path.GetFileName(args.DataFile.FullName));
            List<Snp> snps = null;

            try
            {
                snps = GenotypeDataParser.Parse(args.DataFile.FullName);
            }
            catch (Exception e)
            {
                WriteFatalError(0x1000, "Could not load data file: {0}", e.Message);
            }

            var snpTable = snps
                .GroupBy(x => x.Position)
                .ToDictionary(x => x.Key, x => x.First());

            WriteSuccessMessage("Data file loaded");
            WriteInfoMessage("Loading Y-DNA haplogroup tree");

            Haplogroup root = LoadYDnaTree();
            var mutations = new List<HaplogroupMutation>();

            root.Visit(
                x => mutations.AddRange(
                    x.Mutations.Where(y => !mutations.Any(z => z.Snp == y.Snp))));

            WriteSuccessMessage("Y-DNA haplogroup tree loaded");
            HaplogroupMutation[] snpIndex = mutations
                .Where(x => snpTable.ContainsKey(x.Position))
                .ToArray();

            var matches = snpIndex
                .Where(x =>
                    snpTable.ContainsKey(x.Position) &&
                    snpTable[x.Position].Genotype[0] == x.NewNucleotide)
                .Select(x => FindHaplogroup(x.Haplogroup, root))
                .Where(x => x != null)
                .ToArray();

            var snpIndexTable = snpIndex
                .GroupBy(x => x.Haplogroup)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var matchTable = matches
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x.ToArray());

            var tree2 = LoadYDnaTree()
                .Where(x => x.Mutations.Count == 0 || matchTable.ContainsKey(x.Name))
                .Where(x => Visitor.Any(x, y => matchTable.ContainsKey(y.Name), y => y.Children));

            var stringNodes = Visitor.Select(
                tree2,
                x =>
                {
                    if (!matchTable.ContainsKey(x.Name))
                    {
                        
                        return new StringNode() 
                        { 
                            Value = "~DarkGray~" + x.Name + "~R~" 
                        };
                    }

                    var m = matchTable[x.Name];

                    var value = (float)m.Length / snpIndexTable[x.Name].Length;

                    var color =
                        value >= .75 ? ConsoleColor.Green :
                        value >= .50 ? ConsoleColor.Yellow :
                        ConsoleColor.Red;

                    return new StringNode()
                    {
                        Value = string.Format(
                            "{0}: ~{1}~{2:n2} ({3:n0}/{4:n0})~R~",
                            x.Name,
                            color,
                            value,
                            m.Length,
                            snpIndexTable[x.Name].Length)
                    };
                },
                (p, c) => p.Children.Add(c),
                x => x.Children);

            Cli.WriteLine();
            Cli.WriteSubheader("Haplogroup Matches", "~|Blue~~White~");
            Cli.WriteLine();
            Cli.WriteLine(StringTree.Create(stringNodes, x => x.Value, x => x.Children));

            var matchesGrouped = matches
                .Select(x => new
                {
                    Group = x,
                    Depth = x.GetDepth()
                })
                .OrderByDescending(x => x.Depth)
                .ToArray();

            var deepest = matchesGrouped.First();
            Cli.WriteLine("Deepest match: ~Cyan~{0}~R~", deepest.Group.Name);
        }
    }
}

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

        private const float _nodeMatchThreshold = 0.5F;

        private const float _haplogroupMatchThreshold = 0.5F;

        private Tuple<int, int> CountNodeMatches(
            Dictionary<Haplogroup, Tuple<int, int>> mutationTable,
            Haplogroup child)
        {
            var node = child;

            int count = 0, matches = 0;

            do
            {
                var mutation = mutationTable[node];

                if (mutation.Item2 > 0)
                {
                    count++;

                    if ((float)mutation.Item1 / mutation.Item2 >=
                        _nodeMatchThreshold)
                    {
                        matches++;
                    }
                }
                
                node = node.Parent;
            } while (node != null);

            return Tuple.Create(matches, count);
        }

        private Dictionary<Haplogroup, Tuple<int, int>> GetMutationMatches(
            Haplogroup root,
            Dictionary<int, Snp> snpTable)
        {
            var haplogroupMutations = new Dictionary<Haplogroup, Tuple<int, int>>();

            root.Visit(x => haplogroupMutations.Add(
                x,
                Tuple.Create(
                    x.Mutations
                        .Count(y =>
                            snpTable.ContainsKey(y.Position) &&
                            snpTable[y.Position].Genotype[0] == y.NewNucleotide),
                    x.Mutations.Count(y => snpTable.ContainsKey(y.Position)))));

            return haplogroupMutations;
        }

        private Haplogroup FindHaplogroup(Haplogroup root, Dictionary<int, Snp> snpTable)
        {
            var mutationMatches = GetMutationMatches(root, snpTable);

            var haplogroupMatches = mutationMatches
                .Where(x => x.Value.Item1 != 0)
                .Select(x => new
                {
                    Haplogroup = x.Key,
                    Result = CountNodeMatches(mutationMatches, x.Key),
                })
                .Where(x => (float)x.Result.Item1/ x.Result.Item2 >= _haplogroupMatchThreshold)
                .OrderByDescending(x => x.Result.Item2)
                .ThenByDescending(x => x.Haplogroup.Name);

            return haplogroupMatches.Select(x => x.Haplogroup).FirstOrDefault();
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
            
            var snpIndex = JsonSerializer
                .DeserializeFile<HaplogroupMutation[]>(
                    PathHelper.GetExecutingPath("ydnasnps.json"))
                .Where(x => snpTable.ContainsKey(x.Position))
                .ToArray();

            WriteSuccessMessage("Y-DNA haplogroup tree loaded");
            
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

            var matchRoot = root
                .Where(x => Visitor.Any(x, y => matchTable.ContainsKey(y.Name), y => y.Children));

            var mutationMatches = GetMutationMatches(root, snpTable);

            var stringNodes = Visitor.Select(
                matchRoot,
                x =>
                {
                    var m2 = mutationMatches[x];
                    
                    if (m2.Item2 == 0)
                    {
                        return new StringNode() 
                        { 
                            Value = "~DarkGray~" + x.Name + "~R~" 
                        };
                    }

                    var value = (float)m2.Item1 / m2.Item2 * 100;

                    var color =
                        value >= 75 ? ConsoleColor.Green :
                        value >= 50 ? ConsoleColor.Yellow :
                        ConsoleColor.Red;

                    return new StringNode()
                    {
                        Value = string.Format(
                            "{0}: ~{1}~{2:n0}% ({3:n0}/{4:n0})~R~",
                            x.Name,
                            color,
                            value,
                            m2.Item1,
                            m2.Item2)
                    };
                },
                (p, c) => p.Children.Add(c),
                x => x.Children);

            Cli.WriteLine();
            Cli.WriteSubheader("Haplogroup Matches", "~|Blue~~White~");
            Cli.WriteLine();
            Cli.WriteLine(StringTree.Create(stringNodes, x => x.Value, x => x.Children));
            var haplogroup = FindHaplogroup(root, snpTable);
            Cli.WriteLine("Best match: ~Cyan~{0}~R~", haplogroup.Name);
        }
    }
}

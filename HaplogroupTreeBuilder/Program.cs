using Components;
using Components.ConsolePlus;
using Components.Genetics;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HaplogroupTreeBuilder
{
    class Program
    {
        private static string UnwrapValue(HtmlNode node)
        {
            if (node.Name == "#text")
            {
                return HtmlEntity.DeEntitize(node.InnerText).Trim();
            }
            else
            {
                return string.Join(
                    " ", 
                    node.ChildNodes.Select(UnwrapValue));
            }
        }

        private static int ParsePosition(string position)
        {
            int x;

            if (int.TryParse(position, out x))
            {
                return x;
            }
            else
            {
                return -1;
            }
        }

        private static char[] ParseMutation(string mutation)
        {
            var parts = mutation
                .Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();

            if (parts.Length != 2 ||
                parts.Any(x => x.Length != 1))
            {
                return new char[] { '\0', '\0' };
            }

            return new char[] { parts[0][0], parts[1][0] };
        }

        private static HaplogroupMutation[] ParseSnpIndex(string filename)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(filename);

            return doc.DocumentNode
                .Descendants("table")
                .Single(x =>
                    x.Attributes["class"] != null &&
                    x.Attributes["class"].Value == "bord")
                .Element("tbody")
                .Elements("tr")
                .Skip(1)
                .Select(x => x
                    .Elements("td")
                    .Select(UnwrapValue)
                    .ToArray())
                .Where(x => x.Length != 0)
                .Select(x => new HaplogroupMutation
                {
                    Haplogroup = x[1],
                    Snp = x[0],
                    Rsid = x[3],
                    Position = ParsePosition(x[4]),
                    OldNucleotide = ParseMutation(x[5])[0],
                    NewNucleotide = ParseMutation(x[5])[1],
                })
                .OrderBy(x => x.Haplogroup)
                .ToArray();
        }

        private const char _delim = '-';

        private static Dictionary<string, int> ParseFlatTree(string filename)
        {
            var lines = File
                .ReadAllLines(filename)
                //.Select(x => Regex.Replace(x, @"\s", ""))
                .Select(x => Regex.Replace(x, @"([\u0080-\uffff])\s+", @"$1"))
                .Select(x => Regex.Replace(x, @"[\u0080-\uffff]", _delim.ToString()))
                .Where(x => !x.All(y => y == _delim))
                .Select(x => Regex.Replace(x, @"\s.*", @""))
                .ToArray();

            var groups = lines
                .GroupBy(x => x.TrimStart('-'))
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToArray();

            var table = lines
                .ToDictionary(
                    x => x.TrimStart('-'),
                    x => Regex.Match(x, @"^-*").Value.Length);

            //var least = table
            //    .OrderBy(x => x.Value)
            //    .First()
            //    .Value;

            return table;

            //var doc = new HtmlDocument();
            //doc.Load(filename);
            
            //foreach (var f in doc.DocumentNode.Descendants("font").ToArray())
            //{
            //    var parent = f.ParentNode;
            //    var text = f.Element("#text");

            //    if (text != null)
            //    {
            //        if (text.InnerText.Trim() == "A")
            //        {
            //            Console.WriteLine();
            //        }
            //        text.Remove();
            //        parent.InsertAfter(text, f);
            //    }
                
            //    f.Remove();
            //}

            //var firstGroup = doc.DocumentNode
            //    .Descendants("div")
            //    .Single(x => 
            //        x.Element("b") != null && 
            //        x.Element("b").InnerText == "A");

            //var haplogroups = firstGroup.ParentNode
            //    .Elements("div")
            //    .SkipWhile(x => 
            //        x.Element("b") == null ||
            //        x.Element("b").InnerText != "A")
            //    .Where(x => 
            //        x.ChildNodes.Any() &&
            //        x.ChildNodes.All(y => y.Name == "span" || y.Name == "b"))
            //    .ToArray();

            //var div = doc.DocumentNode
            //    .Descendants("div")
            //    .Single(x =>
            //        x.Attributes.Any(y => y.Name == "id") &&
            //        x.Attributes["id"].Value == "sites-canvas-main-content")
            //    .Descendants("div")
            //    .Where(x => 
            //        x.ChildNodes.Any(y => y.Name == "b") &&
            //        x.ChildNodes.All(y => y.Name == "b" || y.Name == "span"))
            //    .ToArray();

            //var spans = doc.DocumentNode
            //    .Descendants("span")
            //    .Where(x =>
            //        x.Attributes["class"].Value == "light" ||
            //        x.Attributes["class"].Value == "hap")
            //    .Select(x => new
            //    {
            //        IsHaplogroup = x.Attributes["class"].Value == "hap",
            //        Span = x,
            //    });

            //var spanGroups = new List<List<HtmlNode>>();
            //var group = new List<HtmlNode>();

            //foreach (var span in spans)
            //{
            //    group.Add(span.Span);

            //    if (span.IsHaplogroup)
            //    {
            //        spanGroups.Add(group);
            //        group = new List<HtmlNode>();
            //    }
            //}

            //if (group.Any())
            //{
            //    spanGroups.Add(group);
            //}

            //return spanGroups
            //    .ToDictionary(
            //        x => x.Last().InnerText.Trim(),
            //        x => x
            //            .Take(x.Count - 1)
            //            .Select(y => HtmlEntity.DeEntitize(y.InnerText))
            //            .Select(y => Regex.Replace(y, @"\s", ""))
            //            .Select(y => y.Replace("â€¢", "="))
            //            .Count());
        }

        private static Haplogroup ParseTree(string filename)
        {
            var flatTree = ParseFlatTree(filename);
            var stack = new Stack<Haplogroup>();

            stack.Push(new Haplogroup() { Name = flatTree.First().Key });

            Action<string> add = x =>
            {
                var g = new Haplogroup()
                {
                    Name = x,
                    Parent = stack.Peek(),
                };
                stack.Peek().Children.Add(g);
                stack.Push(g);
            };

            var depth = 0;
            var filler = 0;

            foreach (var node in flatTree.Skip(1))
            {
                if (node.Value == depth)
                {
                    stack.Pop();
                    add(node.Key);
                }
                //else if (node.Value == depth + 1)
                else if (node.Value > depth)

                {
                    var delta = node.Value - depth;
                    depth += delta;

                    foreach (var f in Enumerable.Range(0, delta - 1))
                    {
                        add("FILLER_" + filler++);
                    }

                    add(node.Key);
                }
                //else if (node.Value > depth)
                //{
                //    Console.WriteLine();
                //}
                else
                {
                    var delta = depth - node.Value;
                    depth -= delta;

                    for (var i = 0; i < delta + 1; i++)
                    {
                        stack.Pop();
                    }

                    add(node.Key);
                }
            }

            return stack.Last();
        }

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

        private static string Dump(Haplogroup group, bool dumpSnps)
        {
            var sb = new StringBuilder();
            Dump(group, dumpSnps, sb, 0, new int[] { }, true);
            
            return sb.ToString();
        }

        private static void Dump(
            Haplogroup group,
            bool dumpSnps,
            StringBuilder sb, 
            int indentation, 
            int[] siblingDepths, 
            bool lastChild)
        {
            for (int i = 0; i < indentation; i++)
            {
                sb.Append(
                    lastChild && i == indentation - 1 ? "└" :
                    siblingDepths.Any() && siblingDepths.Last() == i && i == indentation - 1 ? "├" :
                    siblingDepths.Any() && siblingDepths.Contains(i) ? "│" :
                    " ");
            }

            sb.AppendLine(
                !dumpSnps || !group.Mutations.Any() ? 
                    group.Name :
                    string.Format(
                        "{0} ({1})", 
                        group.Name,
                        group.Mutations.Select(x => x.Snp).Join(", ")));
            
            foreach (var child in group.Children)
            {
                var lc = child == group.Children.Last();

                Dump(
                    child, 
                    dumpSnps,
                    sb, 
                    indentation + 1,
                    !lc ? 
                        siblingDepths
                            .Concat(new[] { indentation })
                            .ToArray() :
                        siblingDepths,
                    lc);
            }
        }

        private static void RemoveFiller(Haplogroup group)
        {
            if (group.Name.StartsWith("FILLER_"))
            {
                group.Parent.Children.Remove(group);
                group.Parent.Children.AddRange(group.Children);
                foreach (var c in group.Children)
                {
                    c.Parent = group.Parent;
                }
            }

            foreach (var c in group.Children.ToArray())
            {
                RemoveFiller(c);
            }
        }

        private static string GetTreeFile(string filename)
        {
            return PathHelper.GetExecutingPath("trees", filename);
        }

        static void Main(string[] args)
        {
            Cli.WriteHeader("Haplogroup Tree Builder", "~|Blue~~White~");
            Cli.WriteLine("Loading trunk");
            var treePath = PathHelper.GetExecutingPath("trees");
            var trunkFile = GetTreeFile(Path.Combine(treePath, "ydnatree.txt"));
            var trunk = ParseTree(trunkFile);
            var treeFiles = Directory.GetFiles(treePath, "ydnatree_*.txt");
            Cli.WriteLine("Loading ~Cyan~{0}~R~ trees", treeFiles.Length);
            var trees = treeFiles.Select(ParseTree).ToList();
            Cli.WriteLine("Merging trees");
            var lastCount = -1;
            
            while (trees.Any())
            {
                foreach (var childTree in trees.ToArray())
                {
                    var match = FindHaplogroup(childTree.Name, trunk);
                    
                    if (match == null)
                    {
                        continue;
                    }

                    match.Children.AddRange(childTree.Children);
                    foreach (var c in childTree.Children)
                    {
                        c.Parent = match;
                    }

                    trees.Remove(childTree);
                }

                if (lastCount == trees.Count)
                {
                    break;
                }

                lastCount = trees.Count;
            }

            RemoveFiller(trunk);

            if (!trees.Any())
            {
                Cli.WriteLine("All trees merged");
            }
            else
            {
                Cli.WriteLine("~Yellow~Tree merge failed~r~");
            }

            Cli.WriteLine("Loading SNP index");
            var snpIndex = ParseSnpIndex(@"c:\23andme\ISOGG 2015 Y-DNA SNP Index.html");
            
            var snpHaplogroupTable = snpIndex
                .GroupBy(x => x.Haplogroup)
                .ToDictionary(x => x.Key, x => x.ToArray());

            Cli.WriteLine(
                "~Cyan~{0:n0}~R~ SNPs loaded for ~Cyan~{1:n0}~R~ haplogroups", 
                snpIndex.Length,
                snpHaplogroupTable.Count);


            var groupMatches = snpHaplogroupTable
                .Select(x => new
                {
                    Group = FindHaplogroup(x.Key, trunk),
                    Snps = x,
                })
                .Where(x => x.Group != null)
                .ToArray();

            foreach (var g in groupMatches)
            {
                g.Group.Mutations = g.Snps.Value.ToList();
            }

            Cli.WriteLine("Saving trees");
            JsonSerializer.SerializeToFile(@"ydnatree.json", trunk);
            File.WriteAllText(@"ydnatree.txt", Dump(trunk, dumpSnps: true));
            File.WriteAllText(@"ydnatree_nosnps.txt", Dump(trunk, dumpSnps: false));
            Cli.WriteLine("~Green~Done~R~");
        }
    }
}

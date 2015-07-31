using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Components.Genetics
{
    public class Haplogroup
    {
        public string Name { get; set; }

        [ScriptIgnore]
        public Haplogroup Parent { get; set; }

        public List<Haplogroup> Children { get; set; }

        public List<HaplogroupMutation> Mutations { get; set; }

        public Haplogroup()
        {
            Children = new List<Haplogroup>();
            Mutations = new List<HaplogroupMutation>();
        }

        public override string ToString()
        {
            return
                Children.Any() ?
                    string.Format(
                        "{0} -> {1}",
                        Name, string.Join(", ", Children.Select(x => x.Name))) :
                Name;
        }
    }
}

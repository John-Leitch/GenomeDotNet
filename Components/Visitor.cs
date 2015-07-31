using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class Visitor<TElement>
    {
        public static void Visit(
            TElement group,
            Action<TElement> action,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            action(group);

            foreach (var c in getChildren(group))
            {
                Visit(group, action, getChildren);
            }
        }
    }
}

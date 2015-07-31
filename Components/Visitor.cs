using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class Visitor
    {
        public static void Visit<TElement>(
            TElement group,
            Action<TElement> action,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            action(group);

            foreach (var c in getChildren(group).ToArray())
            {
                Visit(c, action, getChildren);
            }
        }

        public static TResult Select<TElement, TResult>(
            TElement element,
            Func<TElement, TResult> selector,
            Action<TResult, TResult> addChild,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            var result = selector(element);
            var children = getChildren(element);

            foreach (var child in children)
            {
                addChild(result, Select(child, selector, addChild, getChildren));
            }

            return result;
        }
    }
}

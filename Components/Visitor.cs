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

        public static TElement Where<TElement>(
            TElement element,
            Func<TElement, bool> predicate,
            Action<TElement, TElement> addChild,
            Action<TElement, TElement> removeChild,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            return Where(element, default(TElement), predicate, addChild, removeChild, getChildren);
        }

        private static TElement Where<TElement>(
            TElement element,
            TElement parent,
            Func<TElement, bool> predicate,
            Action<TElement, TElement> addChild,
            Action<TElement, TElement> removeChild,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            Action<TElement, TElement> filterChild = (p, c) =>
            {
                var filteredChild = Where(
                        c,
                        p,
                        predicate,
                        addChild,
                        removeChild,
                        getChildren);

                if (filteredChild != null &&
                    !filteredChild.Equals(default(TElement)))
                {
                    addChild(p, filteredChild);
                }
            };

            if (predicate(element))
            {
                foreach (var child in getChildren(element).ToArray())
                {
                    removeChild(element, child);
                    filterChild(element, child);
                }

                return element;
            }
            else
            {
                if (parent == null)
                {
                    throw new InvalidOperationException("Cannot filter root.");
                }

                var children = getChildren(element);
                removeChild(parent, element);

                foreach (var child in children)
                {
                    filterChild(parent, child);
                }

                return default(TElement);
            }
        }

        public static bool Any<TElement>(
            TElement element,
            Func<TElement, bool> predicate,
            Func<TElement, IEnumerable<TElement>> getChildren)
        {
            if (predicate(element))
            {
                return true;
            }
            else
            {
                return getChildren(element)
                    .Any(x => Any(x, predicate, getChildren));
            }
        }
    }
}

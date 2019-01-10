using AmSpaceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmSpaceTools.Infrastructure.Extensions
{
    public static class TreeExtensions
    {
        /// <summary>
        /// Can throw a StackOverflowException on huge trees. Preferable to Use Descendants(...)
        /// </summary>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> tree, Func<T, IEnumerable<T>> f) => tree.SelectMany(c => f(c).Flatten(f)).Concat(tree);

        public static IEnumerable<T> Descendants<T>(this IEnumerable<T> tree, Func<T, IEnumerable<T>> f)
        {
            var nodes = new Stack<T>(tree);
            while (nodes.Any())
            {
                var node = nodes.Pop();
                yield return node;
                foreach (var n in f(node)) nodes.Push(n);
            }
        }

        public static T FindParentNode<T>(this T node, IEnumerable<T> tree, Func<T, IEnumerable<T>> f)
        {
            return tree.Descendants(f).First(_ => f(_).Contains(node));
        }
    }
}

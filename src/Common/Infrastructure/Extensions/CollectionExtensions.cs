namespace Common.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class CollectionExtensions
    {
#if DEBUG
        /// <summary> Access the contents of a collection </summary>
        /// <remarks> This will execute the collection</remarks>
        internal static IEnumerable<T> Debug<T>(this IEnumerable<T> collection, Action<IEnumerable<T>> executeOnCollection)
        {
            var local = collection.ToList();
            executeOnCollection(local);

            return local;
        }

        /// <summary> Access the contents of a collection </summary>
        /// <remarks> This will execute the collection</remarks>
        internal static IEnumerable<T> DebugItems<T>(this IEnumerable<T> collection, Action<T> executeOnItem)
        {
            void ExecuteOnCollection(IEnumerable<T> items)
            {
                foreach (var item in items)
                    executeOnItem(item);
            }

            return collection.Debug(ExecuteOnCollection);
        }
#endif
    }
}

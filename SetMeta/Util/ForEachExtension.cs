using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SetMeta.Util
{
    public static class ForEachExtension
    {
        /// <summary>
        /// Allows modification of values ​​in a collection as it is listed.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        public static void IndexerForEach<T>(this IList<T> collection, Action<T> action)
        {
            if (collection == null)
                return;
            if (action == null) throw new ArgumentNullException(nameof(action));

            for (int i = 0; i < collection.Count; i++)
            {
                action(collection[i]);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                return;
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            if (collection == null)
                return;
            if (action == null) throw new ArgumentNullException(nameof(action));

            int n = 0;

            foreach (var item in collection)
            {
                action(item, n++);
            }
        }

        public static void ForEachWithIndexOrUntil<T>(this IEnumerable<T> collection, Action<T, int> action, Func<T, int, bool> until)
        {
            if (collection == null)
                return;
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (until == null) throw new ArgumentNullException(nameof(until));

            int n = 0;

            foreach (var item in collection)
            {
                if (until(item, n))
                    break;

                action(item, n++);
            }
        }

        /// <summary>
        /// Performs the action 'otherwise' if the enum is empty
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="collection"></param>
        /// <param name="action"></param>
        /// <param name="elseAction"></param>
        public static void ForEachElse<T>(this IEnumerable<T> collection, Action<T> action, Action elseAction)
        {
            if (collection == null)
                return;
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (elseAction == null) throw new ArgumentNullException(nameof(elseAction));

            ulong n = 0;

            foreach (var item in collection)
            {
                action(item);
                n++;
            }

            if (n == 0)
                elseAction();
        }

        public static void ForEach<T>(this IEnumerable collection, Action<T> action)
        {
            if (collection == null)
                return;
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (T item in collection)
            {
                action(item);
            }
        }

        public static void ForEach(this DataTable dt, Action<DataRow> action)
        {
            if (dt == null) throw new ArgumentNullException(nameof(dt));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (DataRow dtr in dt.Rows)
            {
                action(dtr);
            }
        }

        public static void ForEach(this DataView dv, Action<DataRowView> action)
        {
            if (dv == null) throw new ArgumentNullException(nameof(dv));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (DataRowView drv in dv)
            {
                action(drv);
            }
        }

        /// <summary>
        /// Returns new vocabulary, obtained by merging the other two
        /// </summary>
        /// <typeparam name="T">Key type</typeparam>
        /// <typeparam name="TV">Value type</typeparam>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static Dictionary<T, TV> Merge<T, TV>(this Dictionary<T, TV> dict1, Dictionary<T, TV> dict2)
        {
            return (new[] { dict1, dict2 }).SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Returns new vocabulary, obtained by merging the other two with ignoring key duplicates
        /// </summary>
        /// <typeparam name="T">Key type</typeparam>
        /// <typeparam name="TV">Value type</typeparam>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static Dictionary<T, TV> MergeNonDuplicates<T, TV>(this Dictionary<T, TV> dict1, Dictionary<T, TV> dict2)
        {
            Dictionary<T, TV> dict = dict1.ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (KeyValuePair<T, TV> kvp in dict2)
            {
                if (!dict1.ContainsKey(kvp.Key))
                {
                    dict[kvp.Key] = kvp.Value;
                }
            }

            return dict;
        }
    }
}
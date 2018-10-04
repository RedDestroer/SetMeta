using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace SetMeta.Util
{
    public static class ListExtension
    {
        /// <summary>
        /// Adds a set of items to the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="items"></param>
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            var lst = list as List<T>;
            if (lst != null)
            {
                lst.AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        /// Adds item to the list if it's not already in it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool AddIfUnique<T>(this IList<T> list, T item)
        {
            bool ret = false;

            if (!list.Contains(item))
            {
                list.Add(item);
                ret = true;
            }

            return ret;
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// Returns a subset [idx...end] from list.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="list"></param>
        /// <param name="idx"></param>
        public static List<T> Sublist<T>(this List<T> list, int idx)
        {
            return list.GetRange(idx, list.Count - idx);
        }

        /// <summary>
        /// From http://stackoverflow.com/questions/564366/generic-list-to-datatable
        /// which also offers for better performance, HyperDescriptor: http://www.codeproject.com/Articles/18450/HyperDescriptor-Accelerated-dynamic-property-acces
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable AsDataTable<T>(this IList<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];

            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item);

                table.Rows.Add(values);
            }

            return table;
        }
    }
}
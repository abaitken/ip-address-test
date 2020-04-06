using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IPAddressGenerator
{
    class TextTableBuilder<T>
    {
        private readonly List<Tuple<string, Func<T, string>>> columns;

        public TextTableBuilder()
        {
            columns = new List<Tuple<string, Func<T, string>>>();
        }

        public void AddColumn(string name, Func<T, string> selector)
        {
            columns.Add(new Tuple<string, Func<T, string>>(name, selector));
        }

        public string Format(List<T> items)
        {
            var result = new StringBuilder();

            int number = items.Count;

            columns.Insert(0, new Tuple<string, Func<T, string>>("#", i => number.ToString()));

            var lengths = GetMaxLengths(items);

            var header = CreateHeader(lengths);
            result.AppendLine(header);
            result.AppendLine(new String('-', header.Length));

            for (int i = 0; i < items.Count; i++)
            {
                number = i + 1;

                var row = CreateRow(items[i], lengths);
                result.AppendLine(row);
            }

            return result.ToString();
        }

        private string CreateRow(T item, List<int> lengths)
        {
            var result = new StringBuilder();

            for (int index = 0; index < columns.Count; index++)
            {
                if (result.Length != 0)
                    result.Append(" | ");
                
                var content = columns[index].Item2(item);
                result.Append(content.PadRight(lengths[index]));
            }

            return result.ToString();
        }

        private string CreateHeader(List<int> lengths)
        {
            var result = new StringBuilder();

            for (int index = 0; index < columns.Count; index++)
            {
                if (result.Length != 0)
                    result.Append(" | ");
                
                var content = columns[index].Item1;
                result.Append(content.PadRight(lengths[index]));
            }

            return result.ToString();
        }

        private List<int> GetMaxLengths(List<T> items)
        {
            return (from column in columns
                    select Math.Max(column.Item1.Length, items.Max(i => column.Item2(i).Length))).ToList();
        }
    }
}

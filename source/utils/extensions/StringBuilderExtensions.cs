using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace TribesOfDust.Utils.Extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends a new indented object to the string builder.
        /// </summary>
        /// 
        /// <param name="sb">The target string builder.</param>
        /// <param name="name">The name of the appended value.</param>
        /// <param name="value">The value of the appended value.</param>
        /// <param name="count">The number of indents to prepend.</param>
        /// <param name="indent">The indent to use.</param>
        /// 
        /// <returns>The string builder with the value indented.</returns>
        public static StringBuilder AppendIndented(this StringBuilder sb, string name, object? value, int count = 1, string indent = "\t")
        {
            sb.AppendLine(name.Indent(count - 1, indent));
            sb.AppendLine((value?.ToString() ?? "None").Indent(count, indent));

            return sb;
        }

        /// <summary>
        /// Appends a new indented enumeration to the string builder.
        /// </summary>
        /// 
        /// <param name="sb">The target string builder.</param>
        /// <param name="name">The name of the appended enumeration.</param>
        /// <param name="values">The values to enumerate and append.</param>
        /// <param name="count">The number of indents to prepend.</param>
        /// <param name="indent">The indent to use.</param>
        /// 
        /// <returns>The string builder with the values indented.</returns>
        public static StringBuilder AppendEnumerable(this StringBuilder sb, string name, IEnumerable values, int count = 1, string indent = "\t")
        {
            sb.AppendLine(name.Indent(count - 1, indent));

            int valueIndex = 1;
            foreach(var value in values)
            {
                if (value is string s)
                {
                    sb.AppendLine($"Value {valueIndex++}".Indent(count, indent));
                    sb.AppendLine(s.Indent(count + 1, indent));
                }
            }

            return sb;
        }

        /// <summary>
        /// Appends a new indented dictionary to the string builder.
        /// </summary>
        /// 
        /// <param name="sb">The target string builder.</param>
        /// <param name="name">The name of the appended enumeration.</param>
        /// <param name="values">The values to append.</param>
        /// <param name="count">The number of indents to prepend.</param>
        /// <param name="indent">The indent to use.</param>
        /// 
        /// <returns>The string builder with the values indented.</returns>
        public static StringBuilder AppendDictionary<K, V>(this StringBuilder sb, string name, IDictionary<K, V> values, int count = 1, string indent = "\t")
        {
            sb.AppendLine(name.Indent(count - 1, indent));

            foreach(var entry in values)
            {
                sb.AppendLine($"{entry.Key}".Indent(count, indent));
                sb.AppendLine((entry.Value?.ToString() ?? "None").Indent(count + 1, indent));
            }

            return sb;
        }
    }
}
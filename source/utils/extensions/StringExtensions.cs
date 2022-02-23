using System.Text;

namespace TribesOfDust.Utils.Extensions
{
    public static class StringExtensions 
    {
        /// <summary>
        /// Indents all lines of the string.
        /// </summary>
        /// <param name="input">The input string to indent.</param>
        /// <param name="count">The number of times to indent.</param>
        /// <param name="indent">The string to use for indentation.</param>
        /// <returns>A new, indented string.</returns>
        public static string Indent(this string input, int count = 1, string indent = "\t") 
        {
            if (count <= 0) return input;

            var builder = new StringBuilder(indent.Length * count).Insert(0, indent, count);
            var final = builder.ToString();

            return final + input.Replace("\n", "\n" + final);
        }
    }
}
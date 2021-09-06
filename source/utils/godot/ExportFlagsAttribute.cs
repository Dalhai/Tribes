using System;
using System.Linq;
using Godot;

namespace TribesOfDust.Utils.Godot
{
    /// <summary>
    /// Helper attributes extending godots <see cref="ExportAttribute"/> to facilitate setting
    /// up flags property hints for enumerations that are used as flags. Allows specifying the
    /// attribute on such attributes in order to enable editing their bits in the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ExportFlagsAttribute : ExportAttribute
    {
        /// <summary>
        /// Initializes a new <see cref="ExportFlagsAttribute"/>.
        /// </summary>
        /// <param name="enumType">The enum type to construct the attribute for.</param>
        public ExportFlagsAttribute(Type enumType)
        : base(PropertyHint.Flags, enumType.IsEnum ? string.Join(",", Enum.GetValues(enumType).OfType<Enum>().Where(value => Convert.ToInt32(value) != 0)) : "Invalid Type")
        {
        }
    }
}
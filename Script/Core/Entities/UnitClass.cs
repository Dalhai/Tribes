
using Godot;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public partial class UnitClass : Resource, IVariant<string>
{
    public override string ToString() => $"Unit: {Key}";
    
    #region Exports

    /// <summary>
    /// The overarching class name of the unit.
    /// </summary>
    [Export]
    public string Key { get; set; } = "Unit";

    /// <summary>
    /// The texture associated with the unit.
    /// </summary>
    [Export(PropertyHint.ResourceType, "Texture2D")]
    public Texture2D? Texture2D;

    #endregion
}
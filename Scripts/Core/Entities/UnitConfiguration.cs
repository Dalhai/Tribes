
using Godot;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public partial class UnitConfiguration : Resource, IConfiguration, IVariant<string>
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
    public Texture2D Texture { get; set; } = null!;

    /// <summary>
    /// The movement costs for this unit.
    /// </summary>
    [Export(PropertyHint.ResourceType, "TileTypeCostTable")]
    public TileTypeCostTable MovementCosts { get; set; } = null!;

    #endregion
}
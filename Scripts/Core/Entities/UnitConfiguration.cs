using Godot;
using Godot.Collections;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public partial class UnitConfiguration : Resource, IVariant<string>
{
    public override string ToString() => $"Unit: {Key}";

    #region Exports

    /// <summary>
    /// The overarching class name of the unit.
    /// </summary>
    [Export]
    public string Key { get; set; } = "Unit";

    /// <summary>
    /// The movement costs for this unit.
    /// </summary>
    [Export(PropertyHint.ResourceType, "TileTypeCostTable")]
    public TileTypeCostTable MovementCosts { get; set; } = null!;

    #endregion
}
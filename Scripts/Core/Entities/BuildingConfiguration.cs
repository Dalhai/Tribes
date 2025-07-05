using Godot;
using Godot.Collections;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public partial class BuildingConfiguration : Resource, IVariant<string>
{
    public override string ToString() => $"Building: {Key}";

    #region Exports

    /// <summary>
    /// The overarching class name of the building.
    /// </summary>
    [Export]
    public string Key { get; set; } = "Building";

    #endregion
}
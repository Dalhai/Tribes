using Godot;
using TribesOfDust.Utils;

namespace TribesOfDust.Core.Entities;

public partial class BuildingConfiguration : Resource, IConfiguration, IVariant<string>
{
    public override string ToString() => $"Building: {Key}";

    #region Exports

    /// <summary>
    /// The overarching class name of the building.
    /// </summary>
    [Export]
    public string Key { get; set; } = "Building";

    /// <summary>
    /// The texture associated with the building.
    /// </summary>
    [Export(PropertyHint.ResourceType, "Texture2D")]
    public Texture2D Texture { get; set; }

    #endregion
}
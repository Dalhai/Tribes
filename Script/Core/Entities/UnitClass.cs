
using Godot;
using TribesOfDust.Hex;
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
    #region Size

    /// <summary>
    /// Gets the scale in x-direction necessary to match the expected width.
    /// </summary>
    public float WidthScaleToExpected => Texture2D != null ? HexConstants.DefaultWidth / Texture2D.GetWidth() : 1.0f;

    /// <summary>
    /// Gets the scale in y-direction necessary to match the expected height.
    /// </summary>
    public float HeightScaleToExpected => Texture2D != null ? HexConstants.DefaultHeight / Texture2D.GetHeight() : 1.0f;

    #endregion
}
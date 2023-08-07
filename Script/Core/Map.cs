using System.Text;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Utils;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

/// <summary>
/// A map is a representation of part of the hex world.
/// A map contains information about the current state of a part of a world and the entities within it.
/// A map contains information about units, tiles, effects and various other things in the world.
/// </summary>
///
/// <remarks>
/// A map can cover a whole world, but doesn't necessarily have to. Maps can be used to partition the
/// world into logical sub-worlds. 
/// </remarks>
public class Map(string name) : IVariant<string>
{
    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Tiles), Tiles)
        .AppendIndented(nameof(Units), Units)
        .ToString();

    #endregion
    #region Data

    public string Name { get;  } = name;
    public IHexLayer<Tile> Tiles { get; } = new HexLayer<Tile>();
    public IHexLayer<Unit> Units { get; } = new HexLayer<Unit>();
    public IHexLayer<Building> Buildings { get; } = new HexLayer<Building>();

    #endregion
    #region Variant

    /// <summary>
    /// The key this asset should be mapped to.
    /// </summary>
    public string Key => Name;

    #endregion
}
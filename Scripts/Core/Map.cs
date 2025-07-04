using System.Text;
using TribesOfDust.Core.Entities;
using TribesOfDust.Gen;
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
    #region Generation

    public bool Generate(IHexLayerGenerator<Tile> generator) => generator.Generate(_tiles);
    public bool Generate(IHexLayerGenerator<Building> generator) => generator.Generate(_buildings);
    public bool Generate(IHexLayerGenerator<Unit> generator) => generator.Generate(_units);
    
    #endregion
    #region Entities

    public bool TryAddEntity(IEntity entity)
    {
        switch (entity)
        {
            case Unit     unit    : return _units.TryAdd(unit.Location, unit);
            case Building building: return _buildings.TryAdd(building.Location, building);
            case Tile     tile    : return _tiles.TryAdd(tile.Location, tile);
        }

        return false;
    }

    public bool TryRemoveEntity(IEntity entity)
    {
        switch (entity)
        {
            case Unit     unit    : return _units.TryRemove(unit.Location);
            case Building building: return _buildings.TryRemove(building.Location);
            case Tile     tile    : return _tiles.TryRemove(tile.Location);
        }

        return false;
    }

    #endregion
    #region Data

    public string Name { get; } = name;

    public IHexLayerView<Tile> Tiles         => _tiles;
    public IHexLayerView<Building> Buildings => _buildings;
    public IHexLayerView<Unit> Units         => _units;

    private readonly IHexLayer<Tile> _tiles         = new HexLayer<Tile>();
    private readonly IHexLayer<Building> _buildings = new HexLayer<Building>();
    private readonly IHexLayer<Unit> _units         = new HexLayer<Unit>();

    #endregion

    #region Variant

    /// <summary>
    /// The key this asset should be mapped to.
    /// </summary>
    public string Key => Name;

    #endregion
}
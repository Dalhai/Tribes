using System.Text;
using System.Collections.Generic;
using TribesOfDust.Core.Controllers;
using TribesOfDust.Core.Entities;
using TribesOfDust.Gen;
using TribesOfDust.Hex;
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

    public bool ApplyGenerator(IHexLayerGenerator<Tile> generator)
    {
        return generator.Generate(_tiles, _placements);
    }
    
    #endregion
    #region Entities

    public Unit Create(UnitConfiguration configuration, AxialCoordinate location, IController owner)
    {
        var placement = new Placement();
        var unit = new Unit(configuration, placement, owner);

        _units.Add(location, unit);
        _placements.Add(unit.Identity, placement);
        placement.Location = location;

        return unit;
    }

    public bool Remove(Unit unit)
    {
        if (_placements.TryGetValue(unit.Identity, out var placement)
            && placement.Location is { } location
            && _units.Get(location) is { Identity: var identity }
            && identity == unit.Identity)
        {
            // The unit has a valid placement registered.
            // The placement has an associated location.
            _units.Remove(location);
            return true;
        }

        return false;
    }

    public Building Create(BuildingConfiguration configuration, AxialCoordinate location, IController? owner)
    {
        var placement = new Placement();
        var building = new Building(configuration, placement, owner);

        _buildings.Add(location, building);
        _placements.Add(building.Identity, placement);
        placement.Location = location;

        return building;
    }

    public bool Remove(Building building)
    {
        if (_placements.TryGetValue(building.Identity, out var placement)
            && placement.Location is { } location
            && _buildings.Get(location) is { Identity: var identity }
            && identity == building.Identity)
        {
            // The unit has a valid placement registered.
            // The placement has an associated location.
            _buildings.Remove(location);
            return true;
        }

        return false;
    }

    public Tile Create(TileConfiguration configuration, AxialCoordinate location)
    {
        var placement = new Placement();
        var tile = new Tile(configuration, placement);

        _tiles.Add(location, tile);
        _placements.Add(tile.Identity, placement);
        placement.Location = location;

        return tile;
    }

    public bool Remove(Tile tile)
    {
        if (_placements.TryGetValue(tile.Identity, out var placement)
            && placement.Location is { } location
            && _tiles.Get(location) is { Identity: var identity }
            && identity == tile.Identity)
        {
            // The unit has a valid placement registered.
            // The placement has an associated location.
            _tiles.Remove(location);
            return true;
        }

        return false;
    }

    #endregion
    #region Data

    public string Name { get; } = name;

    public IHexLayerView<Tile> Tiles => _tiles;
    public IHexLayerView<Unit> Units => _units;
    public IHexLayerView<Building> Buildings => _buildings;

    private readonly IHexLayer<Tile> _tiles = new HexLayer<Tile>();
    private readonly IHexLayer<Unit> _units = new HexLayer<Unit>();
    private readonly IHexLayer<Building> _buildings = new HexLayer<Building>();
    private readonly Dictionary<ulong, Placement> _placements = new();

    #endregion

    #region Variant

    /// <summary>
    /// The key this asset should be mapped to.
    /// </summary>
    public string Key => Name;

    #endregion
}
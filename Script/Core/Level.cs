using System.Text;
using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;
using TribesOfDust.Utils.Extensions;
using TribesOfDust.Core.Units;

namespace TribesOfDust.Core;

public partial class Level : RefCounted
{
    #region Constructors
    
    public Level(Repositories repositories)
    {
        Tiles = new EntityTileStorage<Tile>();
        Units = new EntityTileStorage<Unit>();
        Repositories = repositories;
    }

    #endregion
    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Map), Map)
        .AppendIndented(nameof(Tiles), Tiles)
        .AppendIndented(nameof(Units), Units)
        .ToString();

    #endregion
    #region Storages

    public ITileStorage<Tile> Tiles { get; }
    public ITileStorage<Unit> Units { get; }
    
    #endregion
    #region Repositories
    
    public Repositories Repositories { get; }
    
    #endregion
    #region Map

    /// <summary>
    /// The currently loaded map asset
    /// 
    /// This represents information about the current level in its' base state. The level itself
    /// might have already changed way past what has initially been loaded through this map.
    /// Consider this entity the entry point of the current level, not the current state.
    /// </summary>
    public Map? Map 
    {
        get => _map;
        set 
        {
            // Unload the previous map.

            Tiles.Clear();

            // Load the map and generate tiles.
            // Load the map using the default terrain repository.

            _map = value;
            if (_map is not null)
            {
                _map.Generate(Repositories.Terrains, Tiles);
            }
        }
    }
    private Map? _map;
    
    #endregion
}
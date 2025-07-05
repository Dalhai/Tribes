using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

public partial class MapContext : RefCounted
{
    #region Constructor
    
    public MapContext(Context parent)
    {
        Parent = parent;

        var tilesRepository     = new TileConfigurationRepository();
        var unitsRepository     = new UnitConfigurationRepository();
        var buildingsRepository = new BuildingConfigurationRepository();
        var mapsRepository     = new MapRepository(tilesRepository);

        Repos = new(
            mapsRepository, 
            tilesRepository, 
            unitsRepository, 
            buildingsRepository
        );
        
        Repos.Tiles.Load();
        Repos.Buildings.Load();
        Repos.Units.Load();
        Repos.Maps.Load();
        
        // Initialize sub contexts.
        // Try to load the default map from repository, fall back to empty map if not found
        Map? defaultMap = null;
        if (Repos.Maps.HasVariations("Default"))
        {
            defaultMap = Repos.Maps.GetAsset("Default");
            GD.Print($"Loaded default map '{defaultMap.Name}' with {defaultMap.Tiles.Count} tiles");
        }
        else
        {
            GD.Print("No default map found in repository, creating empty map");
        }
        
        Map = defaultMap ?? new("Default");
        Display = new(Map.Tiles);
    }

    #endregion
    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Map), Map)
        .AppendIndented(nameof(Display), Display)
        .AppendIndented(nameof(Parent), Parent)
        .ToString();

    #endregion
    #region Context

    /// <summary>
    /// The context this navigator belongs to.
    /// The context can be used to navigate the context tree.
    /// </summary>
    public Context Parent { get; }
    
    #endregion
    #region Map
    
    /// <summary>
    /// The currently loaded map.
    /// 
    /// Contains information about all entities and tiles in the map.
    /// Contains information about health, stats and other properties of units.
    /// Contains information about fountains, bases, ruins and effects.
    /// 
    /// In general, anything that is happening on a map is in some capacity represented here.
    /// Note that although everything on the map is represented here, how it is displayed
    /// is handled separately in the display layer.
    /// </summary>
    public readonly Map Map;

    /// <summary>
    /// The first selected entity, if any.
    /// </summary>
    public IEntity? Selected
    {
        get => Selection.FirstOrDefault();
        set
        {
            Selection.Clear(); 
            
            if (value is not null)
                Selection.Add(value);
        }
    }

    /// <summary>
    /// The currently selected entities.
    /// </summary>
    public List<IEntity> Selection { get; } = new();

    #endregion
    #region Repositories
    
    public record Repositories(
        MapRepository Maps,
        TileConfigurationRepository Tiles, 
        UnitConfigurationRepository Units, 
        BuildingConfigurationRepository Buildings
    );

    public Repositories Repos { get; }
    
    #endregion
    #region Display
    
    /// <summary>
    /// All graphical elements of the current level.
    /// 
    /// Provides access to overlays, sprites, rendering settings and many more.
    /// If you need to add anything purely visual, such as overlays and on-map
    /// displays that are strictly local, this is the context to access.
    /// </summary>
    public readonly Display Display;

    #endregion
}
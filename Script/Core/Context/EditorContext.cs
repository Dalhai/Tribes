using System.Linq;
using System.Text;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

public partial class EditorContext : RefCounted
{
    public EditorContext(Context parent)
    {
        Parent = parent;
        Maps   = new(Terrains);
        
        Terrains.Load();
        Classes.Load();
        Maps.Load();

        // Initialize sub contexts.
        Map     = Maps.FirstOrDefault() ?? new ("Default");
        Display = new(Map.Tiles);
    }

    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Map), Map)
        .AppendIndented(nameof(Display), Display)
        .AppendIndented(nameof(Parent), Parent)
        .ToString();

    #endregion

    /// <summary>
    /// The context this navigator belongs to.
    /// The context can be used to navigate the context tree.
    /// </summary>
    public Context Parent { get; }
    

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
    public readonly MapRepository Maps;
    public readonly TerrainRepository Terrains = new();
    public readonly UnitClassRepository Classes = new();

    /// <summary>
    /// All graphical elements of the current level.
    /// 
    /// Provides access to overlays, sprites, rendering settings and many more.
    /// If you need to add anything purely visual, such as overlays and on-map
    /// displays that are strictly local, this is the context to access.
    /// </summary>
    public readonly Display Display;
}
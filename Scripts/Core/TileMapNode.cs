using System.Collections.Generic;
using Godot;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

/// <summary>
/// A dedicated container node for efficiently rendering terrain tiles using TileMapLayer.
/// Supports hex-based coordinate system and integrates with the existing Map data model.
/// </summary>
public partial class TileMapNode : Node2D
{
    private TileMapLayer _terrainLayer;
    private readonly Dictionary<string, TileMapLayer> _overlayLayers = new();
    
    /// <summary>
    /// The terrain layer that contains all terrain tiles.
    /// </summary>
    public TileMapLayer TerrainLayer => _terrainLayer ??= GetTerrainLayer();

    public override void _Ready()
    {
        // Ensure we have a terrain layer
        _terrainLayer = GetTerrainLayer();
        
        // Set up the TileMapLayer for hex layout
        _terrainLayer.TileSet = GD.Load<TileSet>("res://Assets/TileSets/TerrainTileset.tres");
        
        base._Ready();
    }

    /// <summary>
    /// Syncs the TileMap with the provided Map's tile data.
    /// </summary>
    /// <param name="map">The map containing tile data to sync</param>
    public void SyncWithMap(Map map)
    {
        // Clear existing tiles
        TerrainLayer.Clear();
        
        // Add all tiles from the map
        foreach (var (coordinate, tile) in map.Tiles)
        {
            SetTile(coordinate, tile);
        }
    }

    /// <summary>
    /// Sets a tile at the specified hex coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to place the tile</param>
    /// <param name="tile">The tile to place</param>
    public void SetTile(AxialCoordinate hexCoordinate, Tile tile)
    {
        var tileMapCoordinate = HexToTileMapCoordinate(hexCoordinate);
        var tileTypeId = (int)tile.Configuration.Key;
        
        // Set the tile using the TileType enum value as source ID
        TerrainLayer.SetCell(tileMapCoordinate, tileTypeId, Vector2I.Zero);
    }

    /// <summary>
    /// Removes a tile at the specified hex coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to remove the tile</param>
    public void RemoveTile(AxialCoordinate hexCoordinate)
    {
        var tileMapCoordinate = HexToTileMapCoordinate(hexCoordinate);
        TerrainLayer.EraseCell(tileMapCoordinate);
    }

    /// <summary>
    /// Gets the tile at the specified hex coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate to check</param>
    /// <returns>The TileType if a tile exists, Unknown otherwise</returns>
    public TileType GetTileType(AxialCoordinate hexCoordinate)
    {
        var tileMapCoordinate = HexToTileMapCoordinate(hexCoordinate);
        var sourceId = TerrainLayer.GetCellSourceId(tileMapCoordinate);
        
        if (sourceId == -1)
            return TileType.Unknown;
            
        return (TileType)sourceId;
    }

    /// <summary>
    /// Converts a world position to a hex coordinate.
    /// </summary>
    /// <param name="worldPosition">The world position</param>
    /// <returns>The corresponding hex coordinate</returns>
    public AxialCoordinate WorldToHexCoordinate(Vector2 worldPosition)
    {
        // Convert world position to local position relative to this TileMap
        var localPosition = ToLocal(worldPosition);
        
        // Convert to hex coordinate using the tile size-based conversion
        var tileSize = TerrainLayer.TileSet.GetTileSize();
        return HexConversions.WorldToHexCoordinate(tileSize, localPosition);
    }

    /// <summary>
    /// Converts a hex coordinate to a world position.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate</param>
    /// <returns>The corresponding world position</returns>
    public Vector2 HexToWorldPosition(AxialCoordinate hexCoordinate)
    {
        var tileSize = TerrainLayer.TileSet.GetTileSize();
        var localPosition = HexConversions.HexToWorldPosition(tileSize, hexCoordinate);
        return ToGlobal(localPosition);
    }

    /// <summary>
    /// Gets or creates the terrain layer.
    /// </summary>
    private TileMapLayer GetTerrainLayer()
    {
        if (_terrainLayer != null)
            return _terrainLayer;
            
        // Look for existing terrain layer
        foreach (Node child in GetChildren())
        {
            if (child is TileMapLayer layer && child.Name == "TerrainLayer")
            {
                _terrainLayer = layer;
                return _terrainLayer;
            }
        }
        
        // Create new terrain layer if none exists
        _terrainLayer = new TileMapLayer
        {
            Name = "TerrainLayer",
            ZIndex = 1,  // Same as tiles in the original implementation
            // Set to vertical orientation and disable collisions/navigation as per issue requirements
            YSortEnabled = false,
            UseKinematicBodies = false,
            CollisionEnabled = false,
            NavigationEnabled = false
        };
        
        AddChild(_terrainLayer);
        return _terrainLayer;
    }

    /// <summary>
    /// Converts hex coordinates to TileMap coordinates.
    /// For hex grids, this is typically a 1:1 mapping.
    /// </summary>
    private static Vector2I HexToTileMapCoordinate(AxialCoordinate hexCoordinate)
    {
        return new Vector2I(hexCoordinate.Q, hexCoordinate.R);
    }

    /// <summary>
    /// Converts TileMap coordinates to hex coordinates.
    /// </summary>
    private static AxialCoordinate TileMapToHexCoordinate(Vector2I tileMapCoordinate)
    {
        return new AxialCoordinate(tileMapCoordinate.X, tileMapCoordinate.Y);
    }

    #region Overlay Management

    /// <summary>
    /// Gets or creates an overlay layer with the specified name.
    /// </summary>
    /// <param name="layerName">The name of the overlay layer</param>
    /// <param name="zIndex">Optional Z-index for the layer (default: 10)</param>
    /// <returns>The overlay TileMapLayer</returns>
    public TileMapLayer GetOrCreateOverlayLayer(string layerName, int zIndex = 10)
    {
        if (_overlayLayers.TryGetValue(layerName, out var existingLayer))
            return existingLayer;

        // Create new overlay layer
        var overlayLayer = new TileMapLayer
        {
            Name = $"OverlayLayer_{layerName}",
            ZIndex = zIndex, // Configurable Z-index for layer ordering
            YSortEnabled = false,
            UseKinematicBodies = false,
            CollisionEnabled = false,
            NavigationEnabled = false
        };

        // Set the overlay tileset
        overlayLayer.TileSet = GD.Load<TileSet>("res://Assets/TileSets/OverlayTileset.tres");

        AddChild(overlayLayer);
        _overlayLayers[layerName] = overlayLayer;

        return overlayLayer;
    }

    /// <summary>
    /// Sets an overlay tile at the specified hex coordinate with the given color.
    /// Creates a color-specific layer to handle proper color rendering.
    /// </summary>
    /// <param name="layerName">The base name of the overlay layer</param>
    /// <param name="hexCoordinate">The hex coordinate where to place the overlay tile</param>
    /// <param name="color">The color to modulate the overlay tile</param>
    public void SetOverlayTile(string layerName, AxialCoordinate hexCoordinate, Color color)
    {
        // Create a unique layer name that includes the color information
        // Use a hash of the color to avoid very long layer names
        var colorHash = color.ToHtml().GetHashCode().ToString("X8");
        var coloredLayerName = $"{layerName}_C{colorHash}";
        
        // Different overlay types get different z-index ranges
        var baseZIndex = GetOverlayTypeZIndex(layerName);
        var overlayLayer = GetOrCreateOverlayLayer(coloredLayerName, baseZIndex);
        var tileMapCoordinate = HexToTileMapCoordinate(hexCoordinate);
        
        // Set the overlay tile (source ID 0, atlas coordinates 0,0)
        overlayLayer.SetCell(tileMapCoordinate, 0, Vector2I.Zero);
        
        // Set the modulation color for this layer
        overlayLayer.Modulate = color;
    }

    /// <summary>
    /// Removes an overlay tile at the specified hex coordinate.
    /// </summary>
    /// <param name="layerName">The base name of the overlay layer</param>
    /// <param name="hexCoordinate">The hex coordinate where to remove the overlay tile</param>
    /// <param name="color">The color of the overlay tile to remove</param>
    public void RemoveOverlayTile(string layerName, AxialCoordinate hexCoordinate, Color color)
    {
        // Create the same unique layer name that includes the color information
        var colorHash = color.ToHtml().GetHashCode().ToString("X8");
        var coloredLayerName = $"{layerName}_C{colorHash}";
        
        if (_overlayLayers.TryGetValue(coloredLayerName, out var overlayLayer))
        {
            var tileMapCoordinate = HexToTileMapCoordinate(hexCoordinate);
            overlayLayer.EraseCell(tileMapCoordinate);
        }
    }

    /// <summary>
    /// Clears all tiles from the specified overlay layer.
    /// </summary>
    /// <param name="layerName">The name of the overlay layer</param>
    public void ClearOverlayLayer(string layerName)
    {
        if (_overlayLayers.TryGetValue(layerName, out var overlayLayer))
        {
            overlayLayer.Clear();
        }
    }

    /// <summary>
    /// Removes an overlay layer completely.
    /// </summary>
    /// <param name="layerName">The name of the overlay layer to remove</param>
    public void RemoveOverlayLayer(string layerName)
    {
        if (_overlayLayers.TryGetValue(layerName, out var overlayLayer))
        {
            overlayLayer.QueueFree();
            _overlayLayers.Remove(layerName);
        }
    }

    #endregion

    /// <summary>
    /// Gets the appropriate Z-index for different overlay types.
    /// This ensures proper layering of different overlay types.
    /// </summary>
    private static int GetOverlayTypeZIndex(string layerName)
    {
        // Different overlay types get different z-index ranges for proper layering
        return layerName.ToLowerInvariant() switch
        {
            var name when name.Contains("selection") => 15, // Selections on top
            var name when name.Contains("highlight") => 12, // Highlights above movement
            var name when name.Contains("movement") => 11,  // Movement overlays
            var name when name.Contains("hover") => 10,     // Hover effects at base level
            _ => 10 // Default overlay z-index
        };
    }
}
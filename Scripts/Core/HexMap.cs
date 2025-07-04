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
public partial class HexMap : Node2D
{
    private TileMapLayer _terrainLayer;
    private TileMapLayer _overlayLayer;
    private readonly Dictionary<AxialCoordinate, HashSet<Color>> _overlayColors = new();
    
    /// <summary>
    /// The terrain layer that contains all terrain tiles.
    /// </summary>
    public TileMapLayer TerrainLayer => _terrainLayer ??= GetTerrainLayer();

    /// <summary>
    /// The single overlay layer for all overlay effects.
    /// </summary>
    public TileMapLayer OverlayLayer => _overlayLayer ??= GetOverlayLayer();

    public override void _Ready()
    {
        // Ensure we have a terrain layer
        _terrainLayer = GetTerrainLayer();
        
        // Set up the TileMapLayer for hex layout
        _terrainLayer.TileSet = GD.Load<TileSet>("res://Assets/TileSets/TerrainTileset.tres");
        
        // Ensure we have an overlay layer
        _overlayLayer = GetOverlayLayer();
        
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
    /// Gets or creates the single overlay layer.
    /// </summary>
    private TileMapLayer GetOverlayLayer()
    {
        if (_overlayLayer != null)
            return _overlayLayer;
            
        // Look for existing overlay layer
        foreach (Node child in GetChildren())
        {
            if (child is TileMapLayer layer && child.Name == "OverlayLayer")
            {
                _overlayLayer = layer;
                return _overlayLayer;
            }
        }
        
        // Create new overlay layer if none exists
        _overlayLayer = new TileMapLayer
        {
            Name = "OverlayLayer",
            ZIndex = 10,  // Above terrain layer
            YSortEnabled = false,
            UseKinematicBodies = false,
            CollisionEnabled = false,
            NavigationEnabled = false
        };

        // Set the overlay tileset
        _overlayLayer.TileSet = GD.Load<TileSet>("res://Assets/TileSets/OverlayTileset.tres");

        AddChild(_overlayLayer);
        return _overlayLayer;
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
    /// Sets an overlay tile at the specified hex coordinate with the given color.
    /// This replaces any existing overlay at that coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to place the overlay tile</param>
    /// <param name="color">The color to modulate the overlay tile</param>
    public void SetOverlayTile(AxialCoordinate hexCoordinate, Color color)
    {
        var tileMapCoordinate = HexToTileMapCoordinate(hexCoordinate);
        
        // Set the overlay tile (source ID 0, atlas coordinates 0,0)
        OverlayLayer.SetCell(tileMapCoordinate, 0, Vector2I.Zero);
        
        // Store the color for this coordinate
        var colorSet = new HashSet<Color> { color };
        _overlayColors[hexCoordinate] = colorSet;
        
        // Set the modulation color for the entire layer
        // Note: This affects all overlay tiles on this layer
        OverlayLayer.Modulate = color;
    }

    /// <summary>
    /// Removes an overlay tile at the specified hex coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to remove the overlay tile</param>
    public void RemoveOverlayTile(AxialCoordinate hexCoordinate)
    {
        var tileMapCoordinate = HexToTileMapCoordinate(hexCoordinate);
        OverlayLayer.EraseCell(tileMapCoordinate);
        _overlayColors.Remove(hexCoordinate);
        
        // Reset modulation if no tiles remain
        if (_overlayColors.Count == 0)
        {
            OverlayLayer.Modulate = Godot.Colors.White;
        }
    }

    /// <summary>
    /// Clears all overlay colors and tiles.
    /// </summary>
    public void ClearAllOverlays()
    {
        _overlayColors.Clear();
        OverlayLayer.Clear();
        OverlayLayer.Modulate = Godot.Colors.White;
    }

    #endregion

}
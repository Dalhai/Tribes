using System.Collections.Generic;
using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core;

/// <summary>
/// A dedicated container node for efficiently rendering terrain tiles using TileMapLayer.
/// Supports hex-based coordinate system and integrates with the existing Map data model.
/// </summary>
public partial class HexMap : Node2D
{
    #region Properties

    /// <summary>
    /// The terrain layer that contains all terrain tiles.
    /// </summary>
    public TileMapLayer TerrainLayer => _terrainLayer ??= GetTerrainLayer();

    /// <summary>
    /// The single overlay layer for all overlay effects.
    /// </summary>
    public TileMapLayer OverlayLayer => _overlayLayer ??= GetOverlayLayer();

    #endregion

    #region Lifecycle

    public override void _Ready()
    {
        _terrainLayer = GetTerrainLayer();
        _overlayLayer = GetOverlayLayer();

        base._Ready();
    }

    public override void _ExitTree()
    {
        // Clean up event connections when the node is destroyed
        DisconnectFromMap();
        base._ExitTree();
    }

    #endregion

    #region Map Synchronization

    /// <summary>
    /// Connects the HexMap to automatically react to tile changes in the specified Map.
    /// </summary>
    /// <param name="map">The map to connect to</param>
    public void ConnectToMap(Map map)
    {
        // Disconnect from previous map first
        DisconnectFromMap();

        _connectedMap = map;

        // Add all tiles from the map
        foreach (var (coordinate, tile) in map.Tiles)
        {
            SetTile(coordinate, tile);
        }

        // Register event handlers for tile changes
        map.Tiles.Added   += OnTileAdded;
        map.Tiles.Removed += OnTileRemoved;
    }

    /// <summary>
    /// Disconnects the HexMap from the currently connected Map.
    /// </summary>
    public void DisconnectFromMap()
    {
        if (_connectedMap != null)
        {
            // Clear existing tiles
            TerrainLayer.Clear();
            OverlayLayer.Clear();

            _connectedMap.Tiles.Added   -= OnTileAdded;
            _connectedMap.Tiles.Removed -= OnTileRemoved;
            _connectedMap               =  null;
        }
    }

    /// <summary>
    /// Event handler for when a tile is added to the connected Map.
    /// </summary>
    private void OnTileAdded(IHexLayerView<Tile> layer, Tile tile, AxialCoordinate coordinate)
    {
        SetTile(coordinate, tile);
    }

    /// <summary>
    /// Event handler for when a tile is removed from the connected Map.
    /// </summary>
    private void OnTileRemoved(IHexLayerView<Tile> layer, Tile tile, AxialCoordinate coordinate)
    {
        RemoveTile(coordinate);
    }

    #endregion

    #region Tile Management

    /// <summary>
    /// Sets a tile at the specified hex coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to place the tile</param>
    /// <param name="tile">The tile to place</param>
    public virtual void SetTile(AxialCoordinate hexCoordinate, Tile tile)
    {
        var tileMapCoordinate = hexCoordinate.ToOffsetCoordinate();
        var tileType          = tile.Configuration.Key;
        
        // Set the tile using the TileType enum value as source ID
        TerrainLayer.SetCell(tileMapCoordinate, 0, tile.Configuration.AtlasCoordinate);
    }

    /// <summary>
    /// Removes a tile at the specified hex coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to remove the tile</param>
    public virtual void RemoveTile(AxialCoordinate hexCoordinate)
    {
        var tileMapCoordinate = hexCoordinate.ToOffsetCoordinate();
        TerrainLayer.EraseCell(tileMapCoordinate);
    }

    #endregion

    #region Overlay Management

    /// <summary>
    /// Sets an overlay tile at the specified hex coordinate with the given color.
    /// This replaces any existing overlay at that coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to place the overlay tile</param>
    /// <param name="color">The color to modulate the overlay tile</param>
    public void SetOverlayTile(AxialCoordinate hexCoordinate, Color color)
    {
        var tileMapCoordinate = hexCoordinate.ToOffsetCoordinate();

        // Set the overlay tile (source ID 0, atlas coordinates 0,0)
        OverlayLayer.SetCell(tileMapCoordinate, 0, new(5, 0));

        // Set the modulation color for this specific cell only
        var tileData = OverlayLayer.GetCellTileData(tileMapCoordinate);
        if (tileData != null)
        {
            tileData.Modulate *= color;
        }
    }

    /// <summary>
    /// Removes an overlay tile at the specified hex coordinate.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate where to remove the overlay tile</param>
    public void RemoveOverlayTile(AxialCoordinate hexCoordinate)
    {
        var tileMapCoordinate = hexCoordinate.ToOffsetCoordinate();
        OverlayLayer.EraseCell(tileMapCoordinate);
    }

    #endregion

    #region Coordinate Conversion

    /// <summary>
    /// Converts a world position to a hex coordinate.
    /// </summary>
    /// <param name="worldPosition">The world position</param>
    /// <returns>The corresponding hex coordinate</returns>
    public AxialCoordinate WorldToHexCoordinate(Vector2 worldPosition)
    {
        // Use Godot's TileMapLayer to convert world position to tile map coordinates
        var localPosition = TerrainLayer.ToLocal(worldPosition);
        var tileMapCoord  = TerrainLayer.LocalToMap(localPosition);

        // Convert tile map coordinates to our hex coordinates
        return OffsetCoordinate.From(tileMapCoord).ToAxialCoordinate();
    }

    /// <summary>
    /// Converts a hex coordinate to a world position.
    /// </summary>
    /// <param name="hexCoordinate">The hex coordinate</param>
    /// <returns>The corresponding world position</returns>
    public Vector2 HexToWorldPosition(AxialCoordinate hexCoordinate)
    {
        // Convert hex coordinate to tile map coordinates
        var tileMapCoord = hexCoordinate.ToOffsetCoordinate();

        // Use Godot's TileMapLayer to convert tile map coordinates to world position
        var localPosition = TerrainLayer.MapToLocal(tileMapCoord);
        var worldPosition = TerrainLayer.ToGlobal(localPosition);

        return worldPosition;
    }

    #endregion

    #region Private Helpers

    /// <summary>
    /// Gets or creates the terrain layer.
    /// </summary>
    private TileMapLayer GetTerrainLayer()
    {
        if (_terrainLayer != null)
            return _terrainLayer;

        // Create new terrain layer if none exists
        _terrainLayer = new TileMapLayer
        {
            Name   = "TerrainLayer",
            ZIndex = 1, // Same as tiles in the original implementation
            YSortEnabled       = false,
            UseKinematicBodies = false,
            CollisionEnabled   = false,
            NavigationEnabled  = false
        };

        _terrainLayer.TileSet = GD.Load<TileSet>("res://Assets/TileSets/TerrainTileset.tres");

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

        // Create new overlay layer if none exists
        _overlayLayer = new TileMapLayer
        {
            Name               = "OverlayLayer",
            ZIndex             = 10, // Above terrain layer
            YSortEnabled       = false,
            UseKinematicBodies = false,
            CollisionEnabled   = false,
            NavigationEnabled  = false
        };

        _overlayLayer.TileSet = GD.Load<TileSet>("res://Assets/TileSets/OverlayTileset.tres");

        AddChild(_overlayLayer);
        return _overlayLayer;
    }

    #endregion

    #region Private Fields

    private TileMapLayer? _terrainLayer;
    private TileMapLayer? _overlayLayer;
    private Map?          _connectedMap;

    #endregion
}
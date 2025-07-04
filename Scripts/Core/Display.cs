using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

public partial class Display : RefCounted
{
    public Display(IHexLayerView<Tile> tiles)
    {
        this.tiles = tiles;

        // Setup event handlers.
        _onOverlayTileAdded = (overlay, color, coordinate) =>
        {
            // Handle sprite-based overlays (for non-tile entities)
            if (tiles.Get(coordinate) is { } tile)
                AddOverlayColor(tile, color);
                
            // Handle HexMap overlays if available
            HexMapAddOverlayColor(coordinate, color);
        };

        _onOverlayTileRemoved = (overlay, color, coordinate) =>
        {
            // Handle sprite-based overlays (for non-tile entities)
            if (tiles.Get(coordinate) is { } tile)
                RemoveOverlayColor(tile, color);
                
            // Handle HexMap overlays if available
            HexMapRemoveOverlayColor(coordinate, color);
        };
    }

    public override string ToString() => new StringBuilder()
        .AppendEnumerable(nameof(_overlays).Remove('_').Capitalize(), _overlays)
        .ToString();

    public readonly Dictionary<ulong, Sprite2D> Sprites = new();
    public readonly Dictionary<ulong, HashSet<Color>> Colors = new();
    
    // Track overlay colors per coordinate for HexMap (similar to sprite system)
    private readonly Dictionary<AxialCoordinate, HashSet<Color>> _tileMapOverlayColors = new();

    /// <summary>
    /// Optional HexMap for handling overlay tiles on TileMapLayers.
    /// When set, overlays will also be rendered on the HexMap in addition to sprite modulation.
    /// </summary>
    public HexMap? HexMap { get; set; }

    #region Overlays

    /// <summary>
    /// Adds a new overlay to the game overlays.
    /// 
    /// Note that the display is automatically kept up to date with the overlay once
    /// it is registered with the display context. You don't need to add overlays multiple
    /// times. You should, however, remember to unregister overlays once you don't need 
    /// them anymore.
    /// </summary>
    public void AddOverlay(IHexLayerView<Color> overlay)
    {
        if (_overlays.Contains(overlay))
            return;

        // Add existing overlay tiles to sprites
        foreach (var (coordinate, color) in overlay)
        {
            if (tiles.Get(coordinate) is { } tile)
                AddOverlayColor(tile, color);
        }

        // Add existing overlay tiles to HexMap if available
        HexMapSyncOverlay(overlay);

        overlay.Added += _onOverlayTileAdded;
        overlay.Removed += _onOverlayTileRemoved;

        _overlays.Add(overlay);
    }

    /// <summary>
    /// Removes an overlay to the game overlays.
    /// 
    /// After unregistering the overlay, updates to it are not tracked anymore.
    /// You can reregister the overlay at any time though, no need to create a
    /// completely new one.
    /// </summary>
    public void RemoveOverlay(IHexLayerView<Color> overlay)
    {
        if (!_overlays.Contains(overlay))
            return;

        // Remove overlay tiles from sprites
        foreach (var (coordinate, color) in overlay)
        {
            if (tiles.Get(coordinate) is { } tile)
                RemoveOverlayColor(tile, color);
        }

        // Remove overlay tiles from HexMap if available
        HexMapUnsyncOverlay(overlay);

        overlay.Added -= _onOverlayTileAdded;
        overlay.Removed -= _onOverlayTileRemoved;

        _overlays.Remove(overlay);
    }

    private void AddOverlayColor(IIdentifiable identifiable, Color color)
    {
        if (Sprites.TryGetValue(identifiable.Identity, out var sprite)) 
        {
            if (!Colors.TryGetValue(identifiable.Identity, out var colors))
            {
                colors = new();
                Colors.Add(identifiable.Identity, colors);
            }
                
            colors.Add(color);
            
            // Mix the colors evenly
            sprite.Modulate = colors.Count == 0
                ? Godot.Colors.White
                : colors.Select(c => c * 1f / colors.Count).Aggregate((f, c) => f + c);
        }
    }

    private void RemoveOverlayColor(IIdentifiable identifiable, Color color)
    {
        if (Sprites.TryGetValue(identifiable.Identity, out var sprite) &&
            Colors.TryGetValue(identifiable.Identity, out var colors))
        {
            colors.Remove(color);
            
            // Mix the colors evenly
            sprite.Modulate = colors.Count == 0
                ? Godot.Colors.White
                : colors.Select(c => c * 1f / colors.Count).Aggregate((f, c) => f + c);
        }
    }

    #endregion

    #region HexMap Overlay Management

    /// <summary>
    /// Adds an overlay color to the HexMap system with color aggregation.
    /// </summary>
    private void HexMapAddOverlayColor(AxialCoordinate coordinate, Color color)
    {
        if (HexMap == null)
            return;

        // Get or create color set for this coordinate
        if (!_tileMapOverlayColors.TryGetValue(coordinate, out var colors))
        {
            colors = new HashSet<Color>();
            _tileMapOverlayColors[coordinate] = colors;
        }
        
        // Add the color
        colors.Add(color);
        
        // Calculate aggregated color (same logic as sprite system)
        var aggregatedColor = colors.Count == 0
            ? Godot.Colors.White
            : colors.Select(c => c * 1f / colors.Count).Aggregate((f, c) => f + c);
        
        // Set the overlay tile with aggregated color
        HexMap.SetOverlayTile(coordinate, aggregatedColor);
    }

    /// <summary>
    /// Removes an overlay color from the HexMap system with color aggregation.
    /// </summary>
    private void HexMapRemoveOverlayColor(AxialCoordinate coordinate, Color color)
    {
        if (HexMap == null)
            return;

        if (_tileMapOverlayColors.TryGetValue(coordinate, out var colors))
        {
            colors.Remove(color);
            
            if (colors.Count == 0)
            {
                // Remove the overlay tile completely if no colors remain
                _tileMapOverlayColors.Remove(coordinate);
                HexMap.RemoveOverlayTile(coordinate);
            }
            else
            {
                // Update the overlay tile with new aggregated color
                var aggregatedColor = colors.Select(c => c * 1f / colors.Count).Aggregate((f, c) => f + c);
                HexMap.SetOverlayTile(coordinate, aggregatedColor);
            }
        }
    }

    /// <summary>
    /// Syncs an overlay with the HexMap system.
    /// </summary>
    private void HexMapSyncOverlay(IHexLayerView<Color> overlay)
    {
        if (HexMap == null)
            return;

        foreach (var (coordinate, color) in overlay)
        {
            HexMapAddOverlayColor(coordinate, color);
        }
    }

    /// <summary>
    /// Removes an overlay from the HexMap system.
    /// </summary>
    private void HexMapUnsyncOverlay(IHexLayerView<Color> overlay)
    {
        if (HexMap == null)
            return;

        foreach (var (coordinate, color) in overlay)
        {
            HexMapRemoveOverlayColor(coordinate, color);
        }
    }

    #endregion

    /// <summary>
    /// Gets a unique layer name for the given overlay.
    /// This creates a consistent mapping between overlay instances and TileMapLayer names.
    /// </summary>
    private static string GetOverlayLayerName(IHexLayerView<Color> overlay)
    {
        // Use the overlay's hash code to create a unique but consistent layer name
        return $"Overlay_{overlay.GetHashCode()}";
    }

    private readonly IHexLayerView<Tile> tiles;
    private readonly Action<IHexLayerView<Color>, Color, AxialCoordinate> _onOverlayTileAdded;
    private readonly Action<IHexLayerView<Color>, Color, AxialCoordinate> _onOverlayTileRemoved;
    private readonly HashSet<IHexLayerView<Color>> _overlays = new();
}
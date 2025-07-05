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
    }

    public override string ToString() => new StringBuilder()
        .AppendEnumerable(nameof(_overlays).Remove('_').Capitalize(), _overlays)
        .ToString();

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

        // Add existing overlay tiles
        foreach (var (coordinate, color) in overlay)
        {
            if (tiles.Get(coordinate) is { } tile)
                AddOverlayColor(tile, color);
        }

        overlay.Added   += OnOverlayTileAdded;
        overlay.Removed += OnOverlayTileRemoved;

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

        // Remove overlay tiles
        foreach (var (coordinate, color) in overlay)
        {
            if (tiles.Get(coordinate) is { } tile)
                RemoveOverlayColor(tile, color);
        }

        overlay.Added   -= OnOverlayTileAdded;
        overlay.Removed -= OnOverlayTileRemoved;

        _overlays.Remove(overlay);
    }
    
    void OnOverlayTileAdded(IHexLayerView<Color> _, Color color, AxialCoordinate coordinate)
    {
        if (tiles.Get(coordinate) is { } tile)
            AddOverlayColor(tile, color);
    }

    private void AddOverlayColor(IIdentifiable identifiable, Color color)
    {
        if (!_colors.TryGetValue(identifiable.Identity, out var colors))
        {
            colors = new();
            _colors.Add(identifiable.Identity, colors);
        }
            
        colors.Add(color);
        
        // Calculate aggregated color (mix colors evenly)
        var aggregatedColor = colors.Count == 0
            ? Colors.White
            : colors.Select(c => c * 1f / colors.Count).Aggregate((f, c) => f + c);

        // Update HexMap if available and this is a tile
        if (HexMap != null && identifiable is Tile tile)
        {
            HexMap.SetOverlayTile(tile.Location, aggregatedColor);
        }
    }

    void OnOverlayTileRemoved(IHexLayerView<Color> _, Color color, AxialCoordinate coordinate)
    {
        if (tiles.Get(coordinate) is { } tile)
            RemoveOverlayColor(tile, color);
    }

    private void RemoveOverlayColor(IIdentifiable identifiable, Color color)
    {
        if (_colors.TryGetValue(identifiable.Identity, out var colors))
        {
            colors.Remove(color);
            
            if (colors.Count == 0)
            {
                // Remove from dictionary if no colors remain
                _colors.Remove(identifiable.Identity);
                
                // Remove from HexMap if available and this is a tile
                if (HexMap != null && identifiable is Tile tile)
                {
                    HexMap.RemoveOverlayTile(tile.Location);
                }
            }
            else
            {
                // Calculate aggregated color (mix colors evenly)
                var aggregatedColor = colors.Select(c => c * 1f / colors.Count).Aggregate((f, c) => f + c);
                
                // Update HexMap if available and this is a tile
                if (HexMap != null && identifiable is Tile tile)
                {
                    HexMap.SetOverlayTile(tile.Location, aggregatedColor);
                }
            }
        }
    }

    #endregion

    private readonly IHexLayerView<Tile> tiles;
    private readonly Dictionary<ulong, HashSet<Color>> _colors = new();
    private readonly HashSet<IHexLayerView<Color>> _overlays = new();
}
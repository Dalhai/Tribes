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
        _onOverlayTileAdded = (_, color, coordinate) =>
        {
            if (tiles.Get(coordinate) is { } tile)
                AddOverlayColor(tile, color);
        };

        _onOverlayTileRemoved = (_, color, coordinate) =>
        {
            if (tiles.Get(coordinate) is { } tile)
                RemoveOverlayColor(tile, color);
        };
    }

    public override string ToString() => new StringBuilder()
        .AppendEnumerable(nameof(_overlays).Remove('_').Capitalize(), _overlays)
        .ToString();

    public readonly Dictionary<ulong, Sprite2D> Sprites = new();
    public readonly Dictionary<ulong, HashSet<Color>> Colors = new();

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

        foreach (var (coordinate, color) in overlay)
        {
            if (tiles.Get(coordinate) is { } tile)
                AddOverlayColor(tile, color);
        }

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

        foreach (var (coordinate, color) in overlay)
        {
            if (tiles.Get(coordinate) is { } tile)
                RemoveOverlayColor(tile, color);
        }

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

    private readonly IHexLayerView<Tile> tiles;
    private readonly Action<IHexLayerView<Color>, Color, AxialCoordinate> _onOverlayTileAdded;
    private readonly Action<IHexLayerView<Color>, Color, AxialCoordinate> _onOverlayTileRemoved;
    private readonly HashSet<IHexLayerView<Color>> _overlays = new();
}
using Godot;

using System;
using System.Text;
using System.Collections.Generic;

using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

public partial class Display : RefCounted
{
    #region Constructors
        
    public Display(ITileLayerView<Tile> tiles)
    {
        Tiles = tiles;

        // Setup event handlers.
        _onOverlayTileAdded = (_, color, coordinates) => AddOverlayColor(coordinates, color); 
        _onOverlayTileRemoved = (_, color, coordinates) => RemoveOverlayColor(coordinates, color); 
    }

    #endregion
    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendEnumerable(nameof(_overlays).Remove('_').Capitalize(), _overlays)
        .ToString();

    #endregion
    #region Access
        
    public ITileLayerView<Tile> Tiles { get; }
        
    #endregion
    #region Overlays

    /// <summary>
    /// Adds a new overlay to the game overlays.
    /// 
    /// Note that the display is automatically kept up to date with the overlay once
    /// it is registered with the display context. You don't need to add overlays multiple
    /// times. You should, however, remember to unregister overlays once you don't need 
    /// them anymore.
    /// </summary>
    public void AddOverlay(ITileLayerView<Color> overlay) 
    {
        if (_overlays.Contains(overlay))
            return;

        foreach (var entry in overlay)
        {
            AddOverlayColor(entry.Key, entry.Value);
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
    public void RemoveOverlay(ITileLayerView<Color> overlay) 
    {
        if (!_overlays.Contains(overlay))
            return;

        foreach (var entry in overlay)
        {
            RemoveOverlayColor(entry.Key, entry.Value);
        }

        overlay.Added -= _onOverlayTileAdded;
        overlay.Removed -= _onOverlayTileRemoved;

        _overlays.Remove(overlay);
    }

    private void AddOverlayColor(AxialCoordinate coordinates, Color color) 
    {
        Tile? tile = Tiles.Get(coordinates);

        // Only add the overlay color if the tile is not null.
        // Handles the case where the overlay wants to color a tile
        // when the tile is not available in the list of tiles.
        tile?.AddOverlayColor(color);
    }

    private void RemoveOverlayColor(AxialCoordinate coordinates, Color color)
    {
        Tile? tile = Tiles.Get(coordinates);

        // Only add the overlay color if the tile is not null.
        // Handles the case where the overlay wants to color a tile
        // when the tile is not available in the list of tiles.
        tile?.RemoveOverlayColor(color);
    }
        
    #endregion

    private readonly Action<ITileLayerView<Color>, Color, AxialCoordinate> _onOverlayTileAdded;
    private readonly Action<ITileLayerView<Color>, Color, AxialCoordinate> _onOverlayTileRemoved;
    private readonly HashSet<ITileLayerView<Color>> _overlays = new();
}
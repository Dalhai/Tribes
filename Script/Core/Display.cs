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
        
    public Display(ITileStorageView<Tile> tiles)
    {
        Tiles = tiles;

        // Setup event handlers.
        _onOverlayTileAdded = (_, args) => AddOverlayColor(args.Coordinates, args.Item); 
        _onOverlayTileRemoved = (_, args) => RemoveOverlayColor(args.Coordinates, args.Item); 
    }

    #endregion
    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendEnumerable(nameof(_overlays).Remove('_').Capitalize(), _overlays)
        .ToString();

    #endregion
    #region Access
        
    public ITileStorageView<Tile> Tiles { get; }
        
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
    public void AddOverlay(ITileStorageView<Color> overlay) 
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
    public void RemoveOverlay(ITileStorageView<Color> overlay) 
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

    private readonly EventHandler<TileStorageEventArgs<Color>> _onOverlayTileAdded;
    private readonly EventHandler<TileStorageEventArgs<Color>> _onOverlayTileRemoved;
    private readonly HashSet<ITileStorageView<Color>> _overlays = new();
}
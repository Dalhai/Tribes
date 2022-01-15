using Godot;

using System;
using System.Collections.Generic;

using TribesOfDust.Data.Assets;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;


namespace TribesOfDust.Core
{
    public class Display
    {
        public Display(Game game) 
        {
            Game = game;

            // Setup event handlers.

            _onOverlayTileAdded = (sender, args) => AddOverlayColor(args.Coordinates, args.Item); 
            _onOverlayTileRemoved = (sender, args) => RemoveOverlayColor(args.Coordinates, args.Item); 
        }

        /// <summary>
        /// The game this display belongs to.
        /// The game can be used to walk the context tree up.
        /// </summary>
        public readonly Game Game;

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
            if (Game.Level is null)
                return;

            Tile? tile = Game.Level.Tiles.Get(coordinates);

            // Only add the overlay color if the tile is not null.
            // Handles the case where the overlay wants to color a tile
            // when the tile is not available in the list of tiles.
            tile?.AddOverlayColor(color);
        }

        private void RemoveOverlayColor(AxialCoordinate coordinates, Color color)
        {
            if (Game.Level is null)
                return;

            Tile? tile = Game.Level.Tiles.Get(coordinates);

            // Only add the overlay color if the tile is not null.
            // Handles the case where the overlay wants to color a tile
            // when the tile is not available in the list of tiles.
            tile?.RemoveOverlayColor(color);
        }

        private readonly EventHandler<TileStorageEventArgs<Color>> _onOverlayTileAdded;
        private readonly EventHandler<TileStorageEventArgs<Color>> _onOverlayTileRemoved;
        private readonly HashSet<ITileStorageView<Color>> _overlays = new();
    }
}
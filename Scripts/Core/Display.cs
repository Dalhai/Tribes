using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

/// <summary>
/// Manages visual display and overlay rendering for the game.
/// Handles overlay registration, color tracking, and synchronization with HexMap rendering.
/// </summary>
public partial class Display : RefCounted
{
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
    /// <param name="overlay">The overlay layer to add</param>
    public void AddOverlay(IHexLayerView<CoreColor> overlay)
    {
        if (_overlays.Contains(overlay))
            return;

        // Add existing overlay tiles
        foreach (var (coordinate, coreColor) in overlay)
        {
            AddOverlayColor(coordinate, coreColor);
        }

        overlay.Added   += OnOverlayTileAdded;
        overlay.Removed += OnOverlayTileRemoved;

        _overlays.Add(overlay);
    }

    /// <summary>
    /// Removes an overlay from the game overlays.
    /// 
    /// After unregistering the overlay, updates to it are not tracked anymore.
    /// You can reregister the overlay at any time though, no need to create a
    /// completely new one.
    /// </summary>
    /// <param name="overlay">The overlay layer to remove</param>
    public void RemoveOverlay(IHexLayerView<CoreColor> overlay)
    {
        if (!_overlays.Contains(overlay))
            return;

        // Remove overlay tiles
        foreach (var (coordinate, coreColor) in overlay)
        {
            RemoveOverlayColor(coordinate, coreColor);
        }

        overlay.Added   -= OnOverlayTileAdded;
        overlay.Removed -= OnOverlayTileRemoved;

        _overlays.Remove(overlay);
    }
    
    /// <summary>
    /// Handles when an overlay tile is added to a layer.
    /// </summary>
    /// <param name="_">The overlay layer (unused)</param>
    /// <param name="coreColor">The core color of the overlay</param>
    /// <param name="coordinate">The coordinate where the overlay was added</param>
    void OnOverlayTileAdded(IHexLayerView<CoreColor> _, CoreColor coreColor, AxialCoordinate coordinate)
    {
        AddOverlayColor(coordinate, coreColor);
    }

    /// <summary>
    /// Adds an overlay color at the specified coordinate.
    /// Updates the HexMap rendering if available.
    /// </summary>
    /// <param name="coordinate">The coordinate to add the overlay color at</param>
    /// <param name="coreColor">The core color to add</param>
    private void AddOverlayColor(AxialCoordinate coordinate, CoreColor coreColor)
    {
        if (!_colors.TryGetValue(coordinate, out var colors))
        {
            colors = new();
            _colors.Add(coordinate, colors);
        }
            
        colors.Add(coreColor);

        // Update HexMap if available - use the new CoreColor overlay system
        if (HexMap != null)
        {
            HexMap.SetOverlayTile(coordinate, coreColor);
        }
    }

    /// <summary>
    /// Handles when an overlay tile is removed from a layer.
    /// </summary>
    /// <param name="_">The overlay layer (unused)</param>
    /// <param name="coreColor">The core color of the overlay</param>
    /// <param name="coordinate">The coordinate where the overlay was removed</param>
    void OnOverlayTileRemoved(IHexLayerView<CoreColor> _, CoreColor coreColor, AxialCoordinate coordinate)
    {
        RemoveOverlayColor(coordinate, coreColor);
    }

    /// <summary>
    /// Removes an overlay color from the specified coordinate.
    /// Updates the HexMap rendering if available.
    /// </summary>
    /// <param name="coordinate">The coordinate to remove the overlay color from</param>
    /// <param name="coreColor">The core color to remove</param>
    private void RemoveOverlayColor(AxialCoordinate coordinate, CoreColor coreColor)
    {
        if (_colors.TryGetValue(coordinate, out var colors))
        {
            colors.Remove(coreColor);
            
            if (colors.Count == 0)
            {
                // Remove from dictionary if no colors remain
                _colors.Remove(coordinate);
            }
            
            // Update HexMap if available - remove from the specific core color layer
            if (HexMap != null)
            {
                HexMap.RemoveOverlayTile(coordinate, coreColor);
            }
        }
    }

    #endregion

    #region Private Fields
    
    /// <summary>
    /// Dictionary tracking overlay colors by coordinate.
    /// </summary>
    private readonly Dictionary<AxialCoordinate, HashSet<CoreColor>> _colors = new();
    
    /// <summary>
    /// Set of registered overlay layers.
    /// </summary>
    private readonly HashSet<IHexLayerView<CoreColor>> _overlays = new();
    
    #endregion
}
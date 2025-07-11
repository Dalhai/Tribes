using System;
using Godot;
using TribesOfDust.Hex;

namespace TribesOfDust.Core;

public static class HexMapExtensions
{
    /// <summary>
    /// Gets the extents of the map using the HexMap's TileMapLayer for accurate calculations.
    /// </summary>
    ///
    /// <remarks>
    /// To get the extents, the map has to look at every tile within the
    /// map and thus calls can be expensive. Also note that only tiles are
    /// considered, buildings and units are not and might be outside of the
    /// extents.
    /// </remarks>
    /// 
    /// <param name="hexMap">The HexMap to use for coordinate conversion</param>
    /// <param name="map">The map to get extents for</param>
    /// <returns>The rectangular area the map covers.</returns>
    public static Rect2 GetMapExtents(this HexMap hexMap, Map map)
    {
        if (map.Tiles.Count == 0)
            return new Rect2();

        Vector2 minimum = Vector2.Inf;
        Vector2 maximum = -Vector2.Inf;
        
        foreach (var tile in map.Tiles)
        {
            var worldPosition = hexMap.HexToWorldPosition(tile.Key);

            minimum.X = Math.Min(minimum.X, worldPosition.X);
            maximum.X = Math.Max(maximum.X, worldPosition.X);
            minimum.Y = Math.Min(minimum.Y, worldPosition.Y);
            maximum.Y = Math.Max(maximum.Y, worldPosition.Y);
        }

        return new(minimum, maximum - minimum);
    }
}
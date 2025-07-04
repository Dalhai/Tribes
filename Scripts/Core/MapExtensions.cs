using System;
using Godot;
using TribesOfDust.Hex;

namespace TribesOfDust.Core;

public static class MapExtensions
{
    /// <summary>
    /// Gets the extents of the map.
    /// </summary>
    ///
    /// <remarks>
    /// To get the extents, the map has to look at every tile within the
    /// map and thus calls can be expensive. Also note that only tiles are
    /// considered, buildings and units are not and might be outside of the
    /// extents.
    /// </remarks>
    /// 
    /// <returns>The rectangular area the map covers.</returns>
    public static Rect2 GetMapExtents(this Map map)
    {
        Vector2 minimum = Vector2.Inf;
        Vector2 maximum = -Vector2.Inf;
        foreach (var tile in map.Tiles)
        {
            var unitPosition = HexConversions.HexToUnit(tile.Key);
            var x = unitPosition.X * HexConstants.DefaultWidth;
            var y = unitPosition.Y * HexConstants.DefaultHeight;

            minimum.X = Math.Min(minimum.X, x);
            maximum.X = Math.Max(maximum.X, x);
            minimum.Y = Math.Min(minimum.Y, y);
            maximum.Y = Math.Max(maximum.Y, y);
        }

        return new(minimum, maximum - minimum);
    }
}


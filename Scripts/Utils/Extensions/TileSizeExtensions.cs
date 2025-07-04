using Godot;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils.Extensions;

/// <summary>
/// Extension methods for Vector2I that provide hex coordinate conversions
/// based on tile size.
/// </summary>
public static class TileSizeExtensions
{
    /// <summary>
    /// Converts hex coordinates to world position using the specified tile size.
    /// </summary>
    /// <param name="tileSize">The tile size to use for calculations</param>
    /// <param name="hexCoordinate">The hex coordinate to convert</param>
    /// <returns>The world position</returns>
    public static Vector2 HexToWorldPosition(this Vector2I tileSize, AxialCoordinate hexCoordinate)
    {
        var unitPosition = HexConversions.HexToUnit(hexCoordinate);
        
        // Scale the unit position by the actual tile size
        // We use the smaller dimension to maintain proper hex proportions
        var scale = Mathf.Min(tileSize.X, tileSize.Y);
        return unitPosition * scale;
    }
    
    /// <summary>
    /// Converts world position to hex coordinates using the specified tile size.
    /// </summary>
    /// <param name="tileSize">The tile size to use for calculations</param>
    /// <param name="worldPosition">The world position to convert</param>
    /// <returns>The hex coordinate</returns>
    public static AxialCoordinate WorldToHexCoordinate(this Vector2I tileSize, Vector2 worldPosition)
    {
        // Scale down by the actual tile size
        // We use the smaller dimension to maintain proper hex proportions
        var scale = Mathf.Min(tileSize.X, tileSize.Y);
        var unitPosition = worldPosition / scale;
        
        return HexConversions.UnitToHex(unitPosition);
    }
}
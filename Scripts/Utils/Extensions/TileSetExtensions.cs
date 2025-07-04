using Godot;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils.Extensions;

/// <summary>
/// Extension methods for TileSet that provide hex coordinate conversions
/// and utility functions based on the actual tile size.
/// </summary>
public static class TileSetExtensions
{
    /// <summary>
    /// Gets the tile size from the first source in the TileSet.
    /// </summary>
    /// <param name="tileSet">The TileSet to get size from</param>
    /// <returns>The tile size as Vector2i</returns>
    public static Vector2I GetTileSize(this TileSet tileSet)
    {
        if (tileSet.GetSourceCount() == 0)
            return new Vector2I(516, 448); // Fallback to expected size
            
        var sourceId = tileSet.GetSourceId(0);
        var source = tileSet.GetSource(sourceId);
        
        if (source is TileSetAtlasSource atlasSource)
        {
            return atlasSource.TextureRegionSize;
        }
        
        return new Vector2I(516, 448); // Fallback to expected size
    }
    
    /// <summary>
    /// Gets the tile width from the TileSet.
    /// </summary>
    /// <param name="tileSet">The TileSet to get width from</param>
    /// <returns>The tile width</returns>
    public static float GetTileWidth(this TileSet tileSet)
    {
        return tileSet.GetTileSize().X;
    }
    
    /// <summary>
    /// Gets the tile height from the TileSet.
    /// </summary>
    /// <param name="tileSet">The TileSet to get height from</param>
    /// <returns>The tile height</returns>
    public static float GetTileHeight(this TileSet tileSet)
    {
        return tileSet.GetTileSize().Y;
    }
    
    /// <summary>
    /// Gets the tile aspect ratio (width/height) from the TileSet.
    /// </summary>
    /// <param name="tileSet">The TileSet to get ratio from</param>
    /// <returns>The tile aspect ratio</returns>
    public static float GetTileRatio(this TileSet tileSet)
    {
        var size = tileSet.GetTileSize();
        return (float)size.X / size.Y;
    }
    
    /// <summary>
    /// Converts hex coordinates to world position using the TileSet's tile size.
    /// </summary>
    /// <param name="tileSet">The TileSet to use for size calculations</param>
    /// <param name="hexCoordinate">The hex coordinate to convert</param>
    /// <returns>The world position</returns>
    public static Vector2 HexToWorldPosition(this TileSet tileSet, AxialCoordinate hexCoordinate)
    {
        var unitPosition = HexConversions.HexToUnit(hexCoordinate);
        var tileSize = tileSet.GetTileSize();
        
        // Scale the unit position by the actual tile size
        // We use the smaller dimension to maintain proper hex proportions
        var scale = Mathf.Min(tileSize.X, tileSize.Y);
        return unitPosition * scale;
    }
    
    /// <summary>
    /// Converts world position to hex coordinates using the TileSet's tile size.
    /// </summary>
    /// <param name="tileSet">The TileSet to use for size calculations</param>
    /// <param name="worldPosition">The world position to convert</param>
    /// <returns>The hex coordinate</returns>
    public static AxialCoordinate WorldToHexCoordinate(this TileSet tileSet, Vector2 worldPosition)
    {
        var tileSize = tileSet.GetTileSize();
        
        // Scale down by the actual tile size
        // We use the smaller dimension to maintain proper hex proportions
        var scale = Mathf.Min(tileSize.X, tileSize.Y);
        var unitPosition = worldPosition / scale;
        
        return HexConversions.UnitToHex(unitPosition);
    }
}
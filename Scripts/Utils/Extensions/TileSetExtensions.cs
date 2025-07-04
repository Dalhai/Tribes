using Godot;

namespace TribesOfDust.Utils.Extensions;

/// <summary>
/// Extension methods for TileSet that provide basic tile size access.
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
}
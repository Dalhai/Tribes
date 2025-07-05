using Godot;
using TribesOfDust.Core;
using TribesOfDust.Core.Entities;

namespace TribesOfDust.Utils;

public static class NodeExtensions
{
    /// <summary>
    /// Creates a sprite for entities using HexMap's coordinate system for consistent positioning.
    /// Note: Tile entities are skipped as they should be handled by HexMap.
    /// </summary>
    /// <param name="node">The node to add the sprite to</param>
    /// <param name="entity">The entity to create a sprite for</param>
    /// <param name="hexMap">The HexMap to use for coordinate conversion</param>
    /// <returns>The created sprite, or null if entity is a tile</returns>
    public static Sprite2D? CreateSpriteForEntity(this Node2D node, IEntity entity, HexMap hexMap)
    {
        // Skip tiles - they should be handled by HexMap
        if (entity is Tile)
            return null;
            
        if (entity.Location is {} location)
        {
            Sprite2D sprite = new();
            
            // Get tile size for scaling calculations
            var tileSize = hexMap.TerrainLayer.TileSet.GetTileSize();
            
            // TODO (MM): This can currently not be implemented, as we don't have texture on configuration anymore.
            
            // Calculate scale to fit the sprite within the tile
            // var texture = entity.Configuration.Texture;
            // if (texture != null)
            // {
            //     // Scale based on tile size and texture size to fit the entity in the tile
            //     var scaleX = (float)tileSize.X / texture.GetWidth();
            //     var scaleY = (float)tileSize.Y / texture.GetHeight();
            //     var minScale = Mathf.Min(scaleX, scaleY);
            //     sprite.Scale = new Vector2(minScale, minScale);
            // }

            sprite.Centered = true;
            sprite.Position = hexMap.HexToWorldPosition(location);
            // sprite.Texture = texture;
            sprite.Modulate = entity.Owner?.Color ?? Colors.White;

            switch (entity)
            {
                case Building:
                    sprite.Scale *= 0.8f;
                    sprite.ZIndex = 10;
                    break;
                case Unit:
                    sprite.Scale *= 0.5f;
                    sprite.ZIndex = 10;
                    break;
            }

            node.AddChild(sprite);
            return sprite;
        }
        
        return null;
    }
}

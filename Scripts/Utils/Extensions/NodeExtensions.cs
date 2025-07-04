using Godot;
using TribesOfDust.Core;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Utils;

public static class NodeExtensions
{
    /// <summary>
    /// Creates a sprite for entities. 
    /// Note: Tile entities are skipped as they should be handled by TileMapNode.
    /// </summary>
    /// <param name="node">The node to add the sprite to</param>
    /// <param name="context">The map context</param>
    /// <param name="entity">The entity to create a sprite for</param>
    /// <param name="tileSize">The tile size to use for position calculations</param>
    /// <param name="scale">The scale to apply to the sprite</param>
    public static void CreateSpriteForEntity(this Node2D node, MapContext context, IEntity<IConfiguration> entity, Vector2I tileSize, Vector2 scale)
    {
        // Skip tiles - they should be handled by TileMapNode
        if (entity is Tile)
            return;
            
        if (entity.Location is {} location)
        {
            Sprite2D sprite = new();

            sprite.Scale = scale;
            sprite.Centered = true;
            sprite.Position = tileSize.HexToWorldPosition(location);
            sprite.Texture = entity.Configuration.Texture;
            sprite.Modulate = entity.Owner?.Color ?? Colors.White;

            switch (entity)
            {
                case Building:
                    sprite.Scale *= 0.8f;
                    sprite.ZIndex = 10;
                    break;
                case Unit:
                    sprite.Scale *= 0.8f;
                    sprite.ZIndex = 10;
                    break;
            }

            context.Display.Sprites.Add(entity.Identity, sprite);
            node.AddChild(sprite);
        }
    }
}

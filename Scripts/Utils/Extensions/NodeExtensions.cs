using Godot;
using TribesOfDust.Core;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;

namespace TribesOfDust.Utils;

public static class NodeExtensions
{
    /// <summary>
    /// Creates a sprite for entities. 
    /// Note: Tile entities are skipped as they should be handled by TileMapNode.
    /// </summary>
    public static void CreateSpriteForEntity(this Node2D node, MapContext context, IEntity<IConfiguration> entity)
    {
        // Skip tiles - they should be handled by TileMapNode
        if (entity is Tile)
            return;
            
        if (entity.Location is {} location)
        {
            Sprite2D sprite = new();

            float widthScaleToExpected = entity.Configuration.Texture != null
                ? HexConstants.DefaultWidth / entity.Configuration.Texture.GetWidth()
                : 1.0f;
            float heightScaleToExpected = entity.Configuration.Texture != null
                ? HexConstants.DefaultHeight / entity.Configuration.Texture.GetHeight()
                : 1.0f;

            sprite.Scale = new Vector2(widthScaleToExpected, heightScaleToExpected);
            sprite.Centered = true;
            sprite.Position = HexConversions.HexToUnit(location) * HexConstants.DefaultSize;
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

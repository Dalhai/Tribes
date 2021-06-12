using Godot;
using System;
using System.Collections.Generic;
using TribesOfDust.Hex;

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit {};
}

namespace TribesOfDust
{

    public class GameManager : Node2D
    {
        private static readonly Lazy<Texture> TileTexture = new(() => GD.Load<Texture>("res://assets/textures/tile_mountain_1.png"));
        private static readonly Lazy<Texture> DebugTexture = new(() => GD.Load<Texture>("res://assets/textures/debug/circle.png"));

        private static readonly Lazy<Sprite> DebugSpriteTL = new(() => new Sprite() { Centered = true, Texture = DebugTexture.Value, ZIndex = 10 });
        private static readonly Lazy<Sprite> DebugSpriteTR = new(() => new Sprite() { Centered = true, Texture = DebugTexture.Value, ZIndex = 10 });
        private static readonly Lazy<Sprite> DebugSpriteBL = new(() => new Sprite() { Centered = true, Texture = DebugTexture.Value, ZIndex = 10 });
        private static readonly Lazy<Sprite> DebugSpriteBR = new(() => new Sprite() { Centered = true, Texture = DebugTexture.Value, ZIndex = 10 });

        private static readonly Lazy<Sprite> DebugSpriteMP = new(() => new Sprite() { Centered = true, Texture = DebugTexture.Value, ZIndex = 10 });

        private readonly Dictionary<AxialCoordinate<int>, HexTile> Tiles = new ();
        private HexTile? ActiveTile;

        private float Size;

        public override void _Ready()
        {
            AddChild(DebugSpriteTL.Value);
            AddChild(DebugSpriteTR.Value);
            AddChild(DebugSpriteBL.Value);
            AddChild(DebugSpriteBR.Value);
            AddChild(DebugSpriteMP.Value);

            Size = TileTexture.Value.GetWidth() / 2.0f;

            for (int x = 0; x < 10; ++x)
            {
                for (int z = 0; z < 10; ++z)
                {
                    var axialCoordinates = new AxialCoordinate<int>(x, z);
                    var position = HexConversions.HexToWorld(axialCoordinates, Size);

                    var tile = new HexTile
                    {
                        Texture = TileTexture.Value,
                        Coordinates = axialCoordinates,
                        Position = position
                    };

                    AddChild(tile);
                    Tiles.Add(tile.Coordinates, tile);
                }
            }
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion mouseMotion)
            {
                var world = mouseMotion.Position + GetNode<Camera2D>("CameraRoot").Position;
                DebugSpriteMP.Value.Position = world;

                var hex = HexConversions.WorldToHex(world, Size);
                var debug = HexConversions.HexToWorld(hex, Size);

                Vector2 TopLeft = debug + Vector2.Left * Size + Vector2.Up * Size;
                Vector2 TopRight = debug + Vector2.Right * Size + Vector2.Up * Size;
                Vector2 BottomLeft = debug + Vector2.Left * Size + Vector2.Down * Size;
                Vector2 BottomRight = debug + Vector2.Right * Size + Vector2.Down * Size;

                DebugSpriteTL.Value.Position = TopLeft;
                DebugSpriteTR.Value.Position = TopRight;
                DebugSpriteBL.Value.Position = BottomLeft;
                DebugSpriteBR.Value.Position = BottomRight;

                if (ActiveTile?.Coordinates != hex && Tiles.TryGetValue(hex, out HexTile tile))
                {
                    if (ActiveTile is not null)
                    {
                        ActiveTile.Modulate = Colors.White;
                    }

                    ActiveTile = tile;
                    tile.Modulate = Colors.Aqua;
                }
            }
        }
    }
}

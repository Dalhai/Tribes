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

        private readonly Dictionary<AxialCoordinate<int>, HexTile> _tiles = new ();
        private HexTile? _activeTile;

        private float _size;

        public override void _Ready()
        {
            AddChild(DebugSpriteTL.Value);
            AddChild(DebugSpriteTR.Value);
            AddChild(DebugSpriteBL.Value);
            AddChild(DebugSpriteBR.Value);
            AddChild(DebugSpriteMP.Value);

            _size = TileTexture.Value.GetWidth() / 2.0f;

            for (int x = 0; x < 10; ++x)
            {
                for (int z = 0; z < 10; ++z)
                {
                    var axialCoordinates = new AxialCoordinate<int>(x, z);
                    var position = HexConversions.HexToWorld(axialCoordinates, _size);

                    var tile = new HexTile(axialCoordinates,TileType.Open,new())
                    {
                        Texture = TileTexture.Value,
                        Centered = true,
                        Position = position
                    };

                    AddChild(tile);
                    _tiles.Add(tile.Coordinates, tile);
                }
            }
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion)
            {
                var world = GetGlobalMousePosition();
                var hex = HexConversions.WorldToHex(world, _size);
                var debug = HexConversions.HexToWorld(hex, _size);

                Vector2 topLeft = debug + Vector2.Left * _size + Vector2.Up * _size;
                Vector2 topRight = debug + Vector2.Right * _size + Vector2.Up * _size;
                Vector2 bottomLeft = debug + Vector2.Left * _size + Vector2.Down * _size;
                Vector2 bottomRight = debug + Vector2.Right * _size + Vector2.Down * _size;

                DebugSpriteTL.Value.Position = topLeft;
                DebugSpriteTR.Value.Position = topRight;
                DebugSpriteBL.Value.Position = bottomLeft;
                DebugSpriteBR.Value.Position = bottomRight;
                DebugSpriteMP.Value.Position = world;

                if (_activeTile?.Coordinates != hex && _tiles.TryGetValue(hex, out HexTile tile))
                {
                    if (_activeTile is not null)
                    {
                        _activeTile.Modulate = Colors.White;
                    }

                    _activeTile = tile;
                    tile.Modulate = Colors.Aqua;
                }
            }
        }
    }
}

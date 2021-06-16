using Godot;
using System.Collections.Generic;
using TribesOfDust.Hex;

namespace System.Runtime.CompilerServices
{
    internal class IsExternalInit
    {
    };
}

namespace TribesOfDust
{
    public class GameManager : Node2D
    {
        private readonly Dictionary<AxialCoordinate<int>, HexTile> _tiles = new();
        private HexTile? _activeTile;

        public override void _Ready()
        {
            for (int x = 0; x < 10; ++x)
            {
                for (int z = 0; z < 10; ++z)
                {
                    var axialCoordinates = new AxialCoordinate<int>(x, z);
                    var tile = new HexTile(axialCoordinates, TileType.Open);
                    
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
                var hex = HexConversions.WorldToHex(world, HexTile.Size);

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
using Godot;
using System.Collections.Generic;
using TribesOfDust.Hex;
using TribesOfDust.Map;

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
        private Dictionary<AxialCoordinate<int>, HexTile> _tiles = new();
        private HexTile? _activeTile;

        public override void _Ready()
        {
            Dictionary<AxialCoordinate<int>, TileType> tiles = new();
            tiles.Add(new AxialCoordinate<int>(0,0),TileType.Rocks);
            tiles.Add(new AxialCoordinate<int>(1,-1),TileType.Open);
            tiles.Add(new AxialCoordinate<int>(1,0),TileType.Open);
            tiles.Add(new AxialCoordinate<int>(0,1),TileType.Canyon);
            tiles.Add(new AxialCoordinate<int>(-1,1),TileType.Canyon);
            tiles.Add(new AxialCoordinate<int>(-1,0),TileType.Dune);
            tiles.Add(new AxialCoordinate<int>(0,-1),TileType.Tundra);

            MapTemplate mapTemplate = new(tiles);
            _tiles = mapTemplate.Generate();
            foreach (var tile in _tiles)
            {
                AddChild(tile.Value);  
            }

            var assets = TileAsset.LoadAll();
            foreach (var asset in assets)
            {
                string type = asset.Type.ToString();
                string texture = asset.Texture?.ResourcePath ?? "Not assigned";

                GD.Print($"{type}, {texture}");
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
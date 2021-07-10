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
            Dictionary<TileType, TileAsset> assets = new();

            MapTemplate mapTemplate = new(tiles);

            var loadedAssets = TileAsset.LoadAll();
            int current = 0;
            foreach (TileAsset asset in loadedAssets)
            {
                string type = asset.Type.ToString();
                string texture = asset.Texture?.ResourcePath ?? "Not assigned";

                GD.Print($"{type}, {texture}");

                for (int tileIndex = 0; tileIndex < 3; ++tileIndex)
                    tiles.Add(new AxialCoordinate<int>(current, tileIndex), asset.Type);

                if (!assets.ContainsKey(asset.Type))
                {
                    assets.Add(asset.Type, asset);
                }

                current += 1;
            }

            _tiles = mapTemplate.Generate(assets);
            foreach (var tile in _tiles)
            {
                AddChild(tile.Value);
            }
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion)
            {
                var world = GetGlobalMousePosition();
                var hex = HexConversions.WorldToHex(world, TileAsset.ExpectedSize);

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

using System.Collections.Generic;

using Godot;
using GodotJson = Godot.Collections.Dictionary;

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
        private Dictionary<TileType, TileAsset> _assets = new();
        private HexTile? _activeTile;
        private MapTemplate? _mapTemplate;

        public override void _Ready()
        {
            // Try to load the tile assets and the default map template

            LoadTileAssets(_assets);
            Load(out _mapTemplate);

            // Try to generate a map from the map template

            if (_mapTemplate is not null)
            {
                _tiles = _mapTemplate.Generate(_assets);
                foreach (var tile in _tiles)
                {
                    AddChild(tile.Value);
                }
            }

            base._Ready();
        }

        public override void _ExitTree()
        {
            // If we have a map template, save the map template in the file system.
            // We overwrite the existing map template with our new map.

            if (_mapTemplate is not null)
            {
                Save(_mapTemplate);
            }

            base._ExitTree();
        }

        private static void LoadTileAssets(Dictionary<TileType, TileAsset> assets)
        {
            var loadedAssets = TileAsset.LoadAll();
            int current = 0;
            foreach (TileAsset asset in loadedAssets)
            {
                string type = asset.Type.ToString();
                string texture = asset.Texture?.ResourcePath ?? "Not assigned";

                GD.Print($"{type}, {texture}");

                if (!assets.ContainsKey(asset.Type))
                {
                    assets.Add(asset.Type, asset);
                }

                current += 1;
            }
        }

        private static void Save(MapTemplate mapTemplate)
        {
            var targetFile = new File();

            // Try to open the default map file to save our default map.
            Error fileOpenError = targetFile.Open("res://assets/maps/map.template", File.ModeFlags.Write);

            // If opening the file worked, serialize the template map and store it in the file as JSON.
            if (fileOpenError == Error.Ok)
            {
                var serializedMap = mapTemplate.Serialize();
                var jsonMap = JSON.Print(serializedMap);

                targetFile.StoreLine(jsonMap);
                targetFile.Close();
            }
        }

        private static void Load(out MapTemplate? mapTemplate)
        {
            mapTemplate = null;
            var targetFile = new File();

            // Try to open the default map file to load our default map.
            Error fileOpenError = targetFile.Open("res://assets/maps/map.template", File.ModeFlags.Read);

            // If opening the file worked, deserialize the template map.
            if (fileOpenError == Error.Ok)
            {
                var stringMap = targetFile.GetLine();
                var jsonMap = JSON.Parse(stringMap);
                targetFile.Close();

                if (jsonMap.Result is GodotJson json)
                {
                    MapTemplate.TryDeserialize(json, out mapTemplate);
                }
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

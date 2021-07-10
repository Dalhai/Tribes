using System;
using System.Collections.Generic;

using Godot;
using GodotJson = Godot.Collections.Dictionary;

using TribesOfDust.Hex;
using TribesOfDust.Map;
using TribesOfDust.Utils;

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

            LoadTileAssets(assets);
            Load(out MapTemplate? mapTemplate);

            if (mapTemplate is not null)
            {
                _tiles = mapTemplate.Generate(assets);
                foreach (var tile in _tiles)
                {
                    AddChild(tile.Value);
                }
            }
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

            // If opening the file worked, serialize the template map and store it in the file as JSON.
            if (fileOpenError == Error.Ok)
            {
                var stringMap = targetFile.GetLine();
                var jsonMap = JSON.Parse(stringMap);
                targetFile.Close();

                GD.Print(jsonMap.Result.GetType().Name);

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

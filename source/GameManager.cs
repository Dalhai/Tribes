using System;
using Godot;
using TribesOfDust.Data.Assets;
using TribesOfDust.Data.Repositories;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;
using GodotJson = Godot.Collections.Dictionary;

namespace TribesOfDust
{
    public class GameManager : Node2D
    {
        private readonly TerrainRepository _repository;
        private readonly Map _map;
        private readonly TileStorage<Tile> _tiles;

        private Tile? _activeTile = null;

        public GameManager()
        {
            _repository = new TerrainRepository();
            _repository.Load();

            _map = Load();
            _tiles = _map.Generate(_repository);
        }

        public override void _Ready()
        {
            foreach (var tile in _tiles)
            {
                AddChild(tile.Value);
            }

            base._Ready();
        }

        public override void _ExitTree()
        {
            Save(_map);
            base._ExitTree();
        }

        private static void Save(Map map)
        {
            var targetFile = new File();

            // Try to open the default map file to save our default map.
            Godot.Error fileOpenError = targetFile.Open("res://assets/maps/map.template", File.ModeFlags.Write);

            // If opening the file worked, serialize the template map and store it in the file as JSON.
            if (fileOpenError == Godot.Error.Ok)
            {
                var serializedMap = map.Serialize();
                var jsonMap = JSON.Print(serializedMap);

                targetFile.StoreLine(jsonMap);
                targetFile.Close();
            }
        }

        private static Map Load()
        {
            Map? map = null;
            var targetFile = new File();

            // Try to open the default map file to load our default map.
            Godot.Error fileOpenError = targetFile.Open("res://assets/maps/map.template", File.ModeFlags.Read);

            // If opening the file worked, deserialize the template map.
            if (fileOpenError == Godot.Error.Ok)
            {
                var stringMap = targetFile.GetLine();
                var jsonMap = JSON.Parse(stringMap);
                targetFile.Close();

                if (jsonMap.Result is GodotJson json)
                {
                    Map.TryDeserialize(json, out map);
                }
            }

            if (map is null)
            {
                throw new InvalidOperationException("Map could not be loaded properly.");
            }

            return map;
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion && _map is not null)
            {
                var world = GetGlobalMousePosition();
                var hex = HexConversions.WorldToHex(world, Terrain.ExpectedSize);

                if (_activeTile?.Coordinates != hex && _tiles.Contains(hex))
                {
                    if (_activeTile is not null)
                    {
                        _activeTile.Modulate = Colors.White;
                        
                    }

                    _activeTile = _tiles.Get(hex);

                    if (_activeTile is not null)
                    {
                        _activeTile.Modulate = Colors.Aqua;
                    }
                }
            }

            if (inputEvent is InputEventKey)
            {
                if (Input.IsActionPressed("ui_TileTundra")&& _activeTileType!=TileType.Tundra && richTextLabel is not null && _mapTemplate is not null)
                {
                    _activeTileType = TileType.Tundra;
                    richTextLabel.Text = $"{_activeTileType} : {_mapTemplate.TilePool[_activeTileType]}";
                }
                else if (Input.IsActionPressed("ui_TileRocks")&&_activeTileType!=TileType.Rocks)
                {
                    _activeTileType = TileType.Rocks;
                }
                else if (Input.IsActionPressed("ui_TileDunes")&&_activeTileType!=TileType.Dune)
                {
                    _activeTileType = TileType.Dune;
                }
                else if (Input.IsActionPressed("ui_TileOpen")&&_activeTileType!=TileType.Open)
                {
                    _activeTileType = TileType.Open;
                }
            }
            if (Input.IsActionPressed("add_tile"))
            {
                if (_mapTemplate != null && richTextLabel is not null)
                {
                    _mapTemplate.TilePool[_activeTileType]+=1;
                    richTextLabel.Text = $"{_activeTileType} : {_mapTemplate.TilePool[_activeTileType]}";
                }
            }
            if (Input.IsActionPressed("remove_tile"))
            {
                if (_mapTemplate != null && _mapTemplate.TilePool[_activeTileType] > 0 && richTextLabel is not null)
                {
                    _mapTemplate.TilePool[_activeTileType]-=1;
                    richTextLabel.Text = $"{_activeTileType} : {_mapTemplate.TilePool[_activeTileType]}";
                }
            }
        }
    }
}

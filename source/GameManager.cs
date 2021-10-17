using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

using Godot;

using TribesOfDust.Data.Assets;
using TribesOfDust.Data.Repositories;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;
using TribesOfDust.Utils.Collections;

namespace TribesOfDust
{
    public class GameManager : Node2D
    {
        private readonly Map _map;
        private readonly TerrainRepository _repository;
        private readonly TileStorage<Tile> _tiles;

        private Tile? _activeTile = null;
        private TileType _activeTileType = TileType.Tundra;

        private RichTextLabel? _activeTileLabel;
        private RichTextLabel? _availableTileCountLabel;
        private RichTextLabel? _tundraTileCountLabel;
        private RichTextLabel? _duneTileCountLabel;
        private RichTextLabel? _rockTileCountLabel;
        private RichTextLabel? _canyonTileCountLabel;

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

            // Initialize user interface.

            _activeTileLabel = GetNode<RichTextLabel>(NodePathActiveTileLabel);
            _availableTileCountLabel = GetNode<RichTextLabel>(NodePathAvailableTileCountLabel);
            _tundraTileCountLabel = GetNode<RichTextLabel>(NodePathTundraTileCountLabel);
            _duneTileCountLabel = GetNode<RichTextLabel>(NodePathDuneTileCountLabel);
            _rockTileCountLabel = GetNode<RichTextLabel>(NodePathRockTileCountLabel);
            _canyonTileCountLabel = GetNode<RichTextLabel>(NodePathCanyonTileCountLabel);

            UpdateTileCounts();
            UpdateActiveTileType();

            base._Ready();
        }

        public override void _ExitTree()
        {
            Save(_map);
            base._ExitTree();
        }

        private void Save(Map map)
        {
            var targetFile = new File();

            // Try to open the default map file to save our default map.
            Godot.Error fileOpenError = targetFile.Open("res://assets/maps/map.template", File.ModeFlags.Write);

            // If opening the file worked, serialize the template map and store it in the file as JSON.
            if (fileOpenError == Godot.Error.Ok)
            {
                map.Tiles.Clear();
                foreach (var tile in _tiles)
                {
                    _map.Tiles[tile.Key] = tile.Value.Key;
                }

                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    NewLineHandling = NewLineHandling.Entitize
                };

                var str = new StringBuilder();
                using var xml = XmlWriter.Create(str, settings);

                var serializer = new DataContractSerializer(typeof(Map));
                serializer.WriteObject(xml, map);
                xml.Flush();

                targetFile.StoreLine(str.ToString());
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
                using var reader = new System.IO.StringReader(targetFile.GetAsText());
                using var xml = System.Xml.XmlReader.Create(reader);

                var deserializer = new DataContractSerializer(typeof(Map));
                map = deserializer.ReadObject(xml) as Map;
            }

            if (map is null)
            {
                map = new("World", "res://assets/maps/map.template");
            }

            return map;
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion)
            {
                var world = GetGlobalMousePosition();
                var hex = HexConversions.WorldToHex(world, Terrain.ExpectedSize);

                if (_activeTile?.Coordinates != hex && _tiles.Contains(hex))
                {
                    if (_activeTile is not null) _activeTile.Modulate = Colors.White;

                    _activeTile = _tiles.Get(hex);

                    if (_activeTile is not null) _activeTile.Modulate = Colors.Aqua;
                }
            }

            if (Input.IsActionPressed(InputActionIncreaseTileCount))
            {
                _map.TilePool.UpdateOrAdd(_activeTileType, count => count + 1, 1);
            }

            if (Input.IsActionPressed(InputActionDecreaseTileCount))
            {
                _map.TilePool.Update(_activeTileType, count => Math.Max(0, count - 1));
            }

            if (inputEvent is InputEventMouseButton mouseButton)
            {
                // Check left mouse button pressed and add selected tile type.
                if (mouseButton.Pressed && mouseButton.ButtonIndex == 1)
                {
                    var world = GetGlobalMousePosition();
                    var hex = HexConversions.WorldToHex(world, Terrain.ExpectedSize);
                    try
                    {
                        var hexTile = new Tile(hex, _repository.GetAsset(_activeTileType));
                        var tile = _tiles.Get(hex);

                        _tiles.Remove(hex);
                        RemoveChild(tile);

                        _tiles.Add(hexTile.Coordinates, hexTile);
                        AddChild(hexTile);
                    }
                    catch (ArgumentException exception)
                    {
                        GD.PrintErr(exception.Message);
                    }
                }
                // Check right mouse button pressed and add open tile, if the current tile is not of open type.
                // Check right mouse button pressed and remove tile, if the current tile is of open type.
                else if (mouseButton.Pressed && mouseButton.ButtonIndex == 2)
                {
                    var world = GetGlobalMousePosition();
                    var hex = HexConversions.WorldToHex(world, Terrain.ExpectedSize);
                    var tile = _tiles.Get(hex);

                    _tiles.Remove(hex);
                    RemoveChild(tile);

                    if (tile is null || tile.Key != TileType.Open)
                    {
                        try
                        {
                            var hexTile = new Tile(hex, _repository.GetAsset(TileType.Open));
                            _tiles.Add(hexTile.Coordinates, hexTile);
                            AddChild(hexTile);
                        }
                        catch (ArgumentException exception)
                        {
                            GD.PrintErr(exception.Message);
                        }
                    }
                }
            }

            UpdateTileCounts();
            UpdateActiveTileType();
        }

        private void UpdateTileCounts()
        {
            if (_availableTileCountLabel is not null)
            {
                _availableTileCountLabel.Text = $"Available: {_tiles.Count(tile => tile.Value.Key == TileType.Open)}";
            }
            if (_tundraTileCountLabel is not null)
            {
                _tundraTileCountLabel.Text = $"Tundra: {_map.TilePool[TileType.Tundra]}";
            }
            if (_duneTileCountLabel is not null)
            {
                _duneTileCountLabel.Text = $"Dune: {_map.TilePool[TileType.Dune]}";
            }
            if (_rockTileCountLabel is not null)
            {
                _rockTileCountLabel.Text = $"Rock: {_map.TilePool[TileType.Rocks]}";
            }
            if (_canyonTileCountLabel is not null)
            {
                _canyonTileCountLabel.Text = $"Canyon: {_map.TilePool[TileType.Canyon]}";
            }
        }
        private void UpdateActiveTileType()
        {
            if (Input.IsActionPressed(InputActionTundra))
                _activeTileType = TileType.Tundra;
            else if (Input.IsActionPressed(InputActionRock))
                _activeTileType = TileType.Rocks;
            else if (Input.IsActionPressed(InputActionDune))
                _activeTileType = TileType.Dune;
            else if (Input.IsActionPressed(InputActionCanyon))
                _activeTileType = TileType.Canyon;

            if (_activeTileLabel is not null)
            {
                _activeTileLabel.BbcodeText = $"[b]{_activeTileType}[/b]";
            }
        }

        #region Constants

        private const string InputActionTundra = "editor_tile_tundra";
        private const string InputActionDune = "editor_tile_dunes";
        private const string InputActionRock = "editor_tile_rocks";
        private const string InputActionCanyon = "editor_tile_canyon";
        private const string InputActionIncreaseTileCount = "editor_increase_tile_count";
        private const string InputActionDecreaseTileCount = "editor_decrease_tile_count";

        private const string NodePathActiveTileLabel = "CameraRoot/CanvasLayer/EditorMenu/ActiveTileType";
        private const string NodePathAvailableTileCountLabel = "CameraRoot/CanvasLayer/EditorMenu/AvailableTileCount";
        private const string NodePathTundraTileCountLabel = "CameraRoot/CanvasLayer/EditorMenu/TundraTileCount";
        private const string NodePathDuneTileCountLabel = "CameraRoot/CanvasLayer/EditorMenu/DuneTileCount";
        private const string NodePathRockTileCountLabel = "CameraRoot/CanvasLayer/EditorMenu/RockTileCount";
        private const string NodePathCanyonTileCountLabel = "CameraRoot/CanvasLayer/EditorMenu/CanyonTileCount";

        #endregion
    }
}
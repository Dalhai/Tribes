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

        private Tile? _activeTile;
        private TileType _activeTileType = TileType.Tundra;
        private PanelContainer? _activeContainer;

        private Label? _availableTileCountLabel;

        private Label? _tundraTileCountLabel;
        private PanelContainer? _tundraContainer;

        private Label? _rocksTileCountLabel;
        private PanelContainer? _rocksContainer;

        private Label? _dunesTileCountLabel;
        private PanelContainer? _dunesContainer;

        private Label? _canyonTileCountLabel;
        private PanelContainer? _canyonContainer;

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

            _availableTileCountLabel = GetNode<Label>(NodePathAvailableTileCountLabel);
            _tundraTileCountLabel = GetNode<Label>(NodePathTundraTileCountLabel);
            _dunesTileCountLabel = GetNode<Label>(NodePathDunesTileCountLabel);
            _rocksTileCountLabel = GetNode<Label>(NodePathRockTileCountLabel);
            _canyonTileCountLabel = GetNode<Label>(NodePathCanyonTileCountLabel);

            _tundraContainer = GetNode<PanelContainer>(NodePathTundraContainer);
            _dunesContainer = GetNode<PanelContainer>(NodePathDunesContainer);
            _rocksContainer = GetNode<PanelContainer>(NodePathRocksContainer);
            _canyonContainer = GetNode<PanelContainer>(NodePathCanyonContainer);

            _activeContainer = _tundraContainer;

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
                _availableTileCountLabel.Text = _tiles.Count(tile => tile.Value.Key == TileType.Open).ToString();
            }
            if (_tundraTileCountLabel is not null)
            {
                _tundraTileCountLabel.Text = _map.TilePool[TileType.Tundra].ToString();
            }
            if (_dunesTileCountLabel is not null)
            {
                _dunesTileCountLabel.Text = _map.TilePool[TileType.Dunes].ToString();
            }
            if (_rocksTileCountLabel is not null)
            {
                _rocksTileCountLabel.Text = _map.TilePool[TileType.Rocks].ToString();
            }
            if (_canyonTileCountLabel is not null)
            {
                _canyonTileCountLabel.Text = _map.TilePool[TileType.Canyon].ToString();
            }
        }
        private void UpdateActiveTileType()
        {
            if (_activeContainer is not null)
                _activeContainer.SelfModulate = new Color(_activeContainer.SelfModulate, 0.0f);

            if (Input.IsActionPressed(InputActionTundra))
                _activeTileType = TileType.Tundra;
            else if (Input.IsActionPressed(InputActionRock))
                _activeTileType = TileType.Rocks;
            else if (Input.IsActionPressed(InputActionDunes))
                _activeTileType = TileType.Dunes;
            else if (Input.IsActionPressed(InputActionCanyon))
                _activeTileType = TileType.Canyon;

            switch(_activeTileType)
            {
                case TileType.Tundra:
                    if (_tundraContainer is not null)
                    {
                        _tundraContainer.SelfModulate = new Color(_tundraContainer.SelfModulate, 0.3f);
                        _activeContainer = _tundraContainer;
                    }
                    break;

                case TileType.Rocks:
                    if (_rocksContainer is not null)
                    {
                        _rocksContainer.SelfModulate = new Color(_rocksContainer.SelfModulate, 0.3f);
                        _activeContainer = _rocksContainer;
                    }
                    break;

                case TileType.Dunes:
                    if (_dunesContainer is not null)
                    {
                        _dunesContainer.SelfModulate = new Color(_dunesContainer.SelfModulate, 0.3f);
                        _activeContainer = _dunesContainer;
                    }
                    break;

                case TileType.Canyon:
                    if (_canyonContainer is not null)
                    {
                        _canyonContainer.SelfModulate = new Color(_canyonContainer.SelfModulate, 0.3f);
                        _activeContainer = _canyonContainer;
                    }
                    break;

            }
        }

        #region Constants

        private const string InputActionTundra = "editor_tile_tundra";
        private const string InputActionRock = "editor_tile_rocks";
        private const string InputActionDunes = "editor_tile_dunes";
        private const string InputActionCanyon = "editor_tile_canyon";
        private const string InputActionIncreaseTileCount = "editor_increase_tile_count";
        private const string InputActionDecreaseTileCount = "editor_decrease_tile_count";

        private const string NodePathAvailableTileCountLabel = "CameraRoot/CanvasLayer/EditorMenu/List/AvailableTileCount/Count";

        private const string NodePathTundraContainer = "CameraRoot/CanvasLayer/EditorMenu/List/TundraContainer";
        private const string NodePathTundraTileCountLabel = NodePathTundraContainer + "/TundraTileCount/Count";

        private const string NodePathRocksContainer = "CameraRoot/CanvasLayer/EditorMenu/List/RocksContainer";
        private const string NodePathRockTileCountLabel = NodePathRocksContainer + "/RocksTileCount/Count";

        private const string NodePathDunesContainer = "CameraRoot/CanvasLayer/EditorMenu/List/DunesContainer";
        private const string NodePathDunesTileCountLabel = NodePathDunesContainer + "/DunesTileCount/Count";

        private const string NodePathCanyonContainer = "CameraRoot/CanvasLayer/EditorMenu/List/CanyonContainer";
        private const string NodePathCanyonTileCountLabel = NodePathCanyonContainer + "/CanyonTileCount/Count";
        #endregion
    }
}
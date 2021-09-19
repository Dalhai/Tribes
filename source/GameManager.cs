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
        
        private RichTextLabel? _richTextLabel;
        
        private TileType _activeTileType = TileType.Tundra;
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
            
            // Initialize user interface.

            _richTextLabel = GetNode<RichTextLabel>("CameraRoot/CanvasLayer/EditorMenu/ActiveTileType");
            UpdateActiveTileType();

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

            if (inputEvent is InputEventKey && _richTextLabel is not null)
            {
                UpdateActiveTileType();
            }


            if (Input.IsActionPressed(InputActionIncreaseTileCount))
                if (_richTextLabel is not null)
                {
                    _map.TilePool[_activeTileType] += 1;
                    _richTextLabel.Text = $"{_activeTileType} : {_map.TilePool[_activeTileType]}";
                }

            if (Input.IsActionPressed(InputActionDecreaseTileCount))
                if (_map.TilePool[_activeTileType] > 0 && _richTextLabel is not null)
                {
                    _map.TilePool[_activeTileType] -= 1;
                    _richTextLabel.Text = $"{_activeTileType} : {_map.TilePool[_activeTileType]}";
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
                        _tiles.Add(hexTile.Coordinates,hexTile);
                        AddChild(hexTile);
                    }
                    catch(ArgumentException exception)
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
                            _tiles.Add(hexTile.Coordinates,hexTile);
                            AddChild(hexTile);
                        }
                        catch(ArgumentException exception)
                        {
                            GD.PrintErr(exception.Message);
                        }
                    }
                }
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
            
            if (_richTextLabel is not null)
            {
                _richTextLabel.Text = $"{_activeTileType}";
            }
        }

        #region Input Actions

        private const string InputActionDune = "editor_tile_dunes";
        private const string InputActionRock = "editor_tile_rocks";
        private const string InputActionTundra = "editor_tile_tundra";
        private const string InputActionCanyon = "editor_tile_canyon";
        private const string InputActionIncreaseTileCount = "editor_increase_tile_count";
        private const string InputActionDecreaseTileCount = "editor_decrease_tile_count";

        #endregion
    }
}
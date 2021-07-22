using Godot;
using GodotJson = Godot.Collections.Dictionary;

using TribesOfDust.Hex;

namespace TribesOfDust
{
    public class GameManager : Node2D
    {
        private readonly TileAssetRepository _repository = new (TileAsset.LoadAll());
        private readonly HexMapTemplate? _mapTemplate = Load();

        private HexMap? _map;
        private HexTile? _activeTile;

        public override void _Ready()
        {
            // Try to generate a map from the map template

            if (_mapTemplate is not null)
            {
                _map = _mapTemplate.Generate(_repository);
                AddChild(_map);
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

        private static void Save(HexMapTemplate mapTemplate)
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

        private static HexMapTemplate? Load()
        {
            HexMapTemplate? mapTemplate = null;
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
                    HexMapTemplate.TryDeserialize(json, out mapTemplate);
                }
            }

            return mapTemplate;
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent is InputEventMouseMotion && _map is not null)
            {
                var world = GetGlobalMousePosition();
                var hex = HexConversions.WorldToHex(world, TileAsset.ExpectedSize);

                if (_activeTile?.Coordinates != hex && _map.HasTileAt(hex))
                {
                    if (_activeTile is not null)
                    {
                        _activeTile.Modulate = Colors.White;
                    }

                    _activeTile = _map.GetTileAt(hex);

                    if (_activeTile is not null)
                    {
                        _activeTile.Modulate = Colors.Aqua;
                    }
                }
            }
        }
    }
}

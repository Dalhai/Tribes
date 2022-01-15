using Godot;

namespace TribesOfDust.Data.Config
{
    [Tool]
    public class TileConfig : Resource
    {
        public const string DefaultTileConfigResourcePath = "res://config/tile_config.tres";

        /// <summary>
        /// The default tile config, or an empty one if the default doesn't exist.
        /// </summary>
        public static TileConfig Default
        {
            get
            {
                if (_defaultConfig is null)
                {
                    _defaultConfig = GD.Load<TileConfig>(DefaultTileConfigResourcePath);

                    if (_defaultConfig is null)
                    {
                        GD.PushWarning($"Could not find default tile config at '{DefaultTileConfigResourcePath}'.");
                        _defaultConfig = new ();
                    }
                }

                return _defaultConfig;
            }
        }

        private static TileConfig? _defaultConfig;

        [Export]
        /// <summary>
        /// The material to be used by tiles using this tile configuration.
        /// </summary>
        public ShaderMaterial? Material;
    }
}
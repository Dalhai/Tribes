using System.Collections.Generic;
using Godot;

namespace TribesOfDust.Hex
{
    public class TileAsset : Resource
    {
        /// <summary>
        /// The default resource path used for tile assets.
        /// </summary>
        public static readonly string Path = "res://assets/tiles";

        /// <summary>
        /// Loads all tile assets in the path specified by <see cref="TileAsset.Path"/>.
        /// </summary>
        /// <returns>The loaded tile assets, if available, or an empty list otherwise.</returns>
        public static List<TileAsset> LoadAll()
        {
            var dir = new Directory();
            var err = dir.Open(Path);

            if (err != Error.Ok)
            {
                return new();
            }

            err = dir.ListDirBegin(skipNavigational: true);
            if (err != Error.Ok)
            {
                return new();
            }

            var results = new List<TileAsset>();
            string name = dir.GetNext();
            while (!string.IsNullOrEmpty(name))
            {
                results.Add(GD.Load<TileAsset>($"{Path}/{name}"));
                name = dir.GetNext();
            }

            return results;
        }

        /// <summary>
        /// The overarching type the tile belongs to.
        /// </summary>
        [Export(PropertyHint.Enum)]
        public TileType Type;

        /// <summary>
        /// The texture associated with the tile.
        /// </summary>
        [Export(PropertyHint.ResourceType, "Texture")]
        public Texture? Texture;
    }
}
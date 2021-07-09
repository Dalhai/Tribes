using System.Collections.Generic;
using Godot;
using TribesOfDust.Utils;

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
        /// <exception cref="GodotException">Thrown when the default directory can't be opened.</exception>
        /// <exception cref="GodotException">Thrown when the default directory can't be iterated.</exception>
        /// <returns>The loaded tile assets, if available, or an empty list otherwise.</returns>
        public static List<TileAsset> LoadAll()
        {
            var dir = new Directory();

            // Open our default directory and abort immediately if we get
            // an error.

            var err = dir.Open(Path);
            if (err != Error.Ok)
            {
                throw new GodotException(err);
            }

            // Start directory iteration and abort immediately if we get
            // an error.

            err = dir.ListDirBegin(skipNavigational: true);
            if (err != Error.Ok)
            {
                throw new GodotException(err);
            }

            // Iterate through the directory entries and try to load the
            // corresponding tile assets.

            var results = new List<TileAsset>();
            var name = dir.GetNext();
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
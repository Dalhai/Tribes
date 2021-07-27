using System.Collections.Generic;

using Godot;

using TribesOfDust.Utils;

namespace TribesOfDust.Hex
{
    public class TileAsset : Resource
    {
        public static readonly float ExpectedSize = 100.0f;
        public static readonly float ExpectedWidth = 2.0f * ExpectedSize;
        public static readonly float ExpectedHeight = 2.0f * Mathf.Sqrt(3.0f / 4.0f * ExpectedSize * ExpectedSize);
        public static readonly float ExpectedRatio = ExpectedWidth / ExpectedHeight;

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
                var asset = GD.Load<TileAsset>($"{Path}/{name}");
                if (asset != null && asset.Texture != null)
                {
                    float width = asset.Texture.GetWidth();
                    float height = asset.Texture.GetHeight();
                    float ratio = width / height;

                    if(!Mathf.IsEqualApprox(ratio, ExpectedRatio))
                    {
                        GD.PushWarning
                        (
                            $"Tile: {asset.ResourcePath} has invalid ratio:\n" +
                            $"Expected Ratio: {ExpectedRatio}\n" +
                            $"Actual Ratio: {ratio}"
                        );
                    }

                    results.Add(asset);
                }

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

        [Export] public bool BlockedNW = false;
        [Export] public bool BlockedN  = false;
        [Export] public bool BlockedNE = false;
        [Export] public bool BlockedSE = false;
        [Export] public bool BlockedS  = false;
        [Export] public bool BlockedSW = false;

        /// <summary>
        /// Gets the scale in x-direction necessary to match the expected width.
        /// </summary>
        public float WidthScaleToExpected => Texture != null ? ExpectedWidth / Texture.GetWidth() : 1.0f;

        /// <summary>
        /// Gets the scale in y-direction necessary to match the expected height.
        /// </summary>
        public float HeightScaleToExpected => Texture != null ? ExpectedHeight / Texture.GetHeight() : 1.0f;
    }
}
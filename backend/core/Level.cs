using System.Text;

using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core
{
    public class Level
    {
        public Level(Repositories repositories)
        {
            Tiles = new TileStorage<Tile>();
            Repositories = repositories;
        }

        #region Overrides

        public override string ToString() => new StringBuilder()
            .AppendIndented(nameof(Map), Map)
            .AppendIndented(nameof(Tiles), Tiles)
            .ToString();

        #endregion

        /// <summary>
        /// The base terrain of the level.
        ///
        /// This is the most basic form of looking at a level. The terrain storage also describes
        /// in essence, where things can be placed on the map. A unit can't (or shouldn't) be placed
        /// at coordinates that are not assigned a terrain, you won't get a rendered tile if the
        /// corresponding terrain is not assigned etc.
        /// </summary>
        public ITileStorage<Tile> Tiles { get; }
        public Repositories Repositories { get; }

        /// <summary>
        /// The currently loaded map asset
        /// 
        /// This represents information about the current level in its' base state. The level itself
        /// might have already changed way past what has initially been loaded through this map.
        /// Consider this entity the entry point of the current level, not the current state.
        /// </summary>
        public Map? Map 
        {
            get => _map;
            set 
            {
                // Unload the previous map.

                Tiles.Clear();

                // Load the map and generate tiles.
                // Load the map using the default terrain repository.

                _map = value;
                if (_map is not null)
                {
                    _map.Generate(Repositories.Terrains, Tiles);
                }
            }
        }
        private Map? _map;
    }
}
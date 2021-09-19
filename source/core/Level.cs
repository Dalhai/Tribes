using TribesOfDust.Data.Assets;
using TribesOfDust.Hex.Storage;

namespace TribesOfDust.Core
{
    public class Level
    {
        public Level(ITileStorage<Terrain> terrain)
        {
            Terrain = terrain;
        }

        /// <summary>
        /// The base terrain of the level.
        ///
        /// This is the most basic form of looking at a level. The terrain storage also describes
        /// in essence, where things can be placed on the map. A unit can't (or shouldn't) be placed
        /// at coordinates that are not assigned a terrain, you won't get a rendered tile if the
        /// corresponding terrain is not assigned etc.
        /// </summary>
        public ITileStorage<Terrain> Terrain { get; init; }
    }
}
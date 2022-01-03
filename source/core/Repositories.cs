using TribesOfDust.Data.Assets;
using TribesOfDust.Data.Repositories;

namespace TribesOfDust.Core
{
    public class Repositories
    {
        public Repositories(Game game)
        {
            Game = game;

            // Load all assets in all repositories immediately.

            Terrain.Load();
        }

        /// <summary>
        /// The game these repositories belong to.
        /// The game can be used to walk the context tree up.
        /// </summary>
        public readonly Game Game;

        /// <summary>
        /// The terrain repository for the current game.
        /// 
        /// Contains all Terrain assets in the default Terrain resource location.
        /// Can be used to grab Terrain information necessary to construct hex tiles.
        /// </summary>
        public readonly TerrainRepository Terrain = new();
    }
}
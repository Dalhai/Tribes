using System.Linq;
using System.Collections.Generic;

namespace TribesOfDust.Core
{
    public class Session
    {
        public Session(Game game) => Game = game;

        /// <summary>
        /// The game these repositories belong to.
        /// The game can be used to walk the context tree up.
        /// </summary>
        public readonly Game Game;
    }
}
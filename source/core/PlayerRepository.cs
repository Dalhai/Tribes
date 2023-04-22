using System.Collections.Generic;
using TribesOfDust.Utils;

namespace TribesOfDust.Core
{
    public partial class PlayerRepository : Repository<string, Player>
    {
        /// <summary>
        /// The default resource path used for tile assets.
        /// </summary>
        private static readonly string DefaultPath = "res://assets/players";
        protected override List<Player> LoadAll() => LoadAll(DefaultPath);

        protected override bool TryLoad(string resourcePath, out Player? asset)
        {
            asset = null;
            return false;
        }
    }
}
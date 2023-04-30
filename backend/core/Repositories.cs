using System.Text;
using Godot;
using TribesOfDust.Hex;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core;

public class Repositories
{
    public Repositories()
    {
        // Load all assets in all repositories immediately.
        Terrains.Load();
        Maps.Load();
    }

    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Terrains), Terrains)
        .AppendIndented(nameof(Maps), Maps)
        .ToString();

    #endregion

    /// <summary>
    /// The terrain repository for the current game.
    /// 
    /// Contains all Terrain assets in the default Terrain resource location.
    /// Can be used to grab Terrain information necessary to construct hex tiles.
    /// </summary>
    public readonly TerrainRepository Terrains = new();

    /// <summary>
    /// The maps repository for the current game.
    /// 
    /// Contains all map assets in the default map resource location.
    /// Can be used to grab map information necessary to load different levels.
    /// </summary>
    public readonly MapRepository Maps = new();
}
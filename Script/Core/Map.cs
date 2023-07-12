using System.Text;
using TribesOfDust.Hex.Storage;
using TribesOfDust.Utils.Extensions;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Utils;

namespace TribesOfDust.Core;

/// <summary>
/// A map is a representation of part of the hex world.
/// A map contains information about the current state of a part of a world and the entities within it.
/// A map contains information about units, tiles, effects and various other things in the world.
/// </summary>
///
/// <remarks>
/// A map can cover a whole world, but doesn't necessarily have to. Maps can be used to partition the
/// world into logical sub-worlds. 
/// </remarks>
public class Map: IEntity, IVariant<string>
{
    #region Constructors
    
    public Map(string name)
    {
        Name = name;
        Identity = Identities.GetNextIdentity();
        Hexes = new HexLayer<Tile>();
        Units = new HexLayer<Unit>();
    }

    #endregion
    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Hexes), Hexes)
        .AppendIndented(nameof(Units), Units)
        .ToString();

    #endregion
    #region Data

    public string Name { get;  }
    public IHexLayer<Tile> Hexes { get; }
    public IHexLayer<Unit> Units { get; }
    
    #endregion
    #region Entity
    
    public ulong Identity { get; }
    
    #endregion
    #region Variant

    public string Key => Name;

    #endregion
}
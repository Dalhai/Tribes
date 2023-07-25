using System.Text;
using TribesOfDust.Core.Controllers;
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
        Owner = null;
        
        Identity = Identities.GetNextIdentity();
        
        Tiles = new HexLayer<Tile>();
        Units = new HexLayer<Unit>();
    }

    #endregion
    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Tiles), Tiles)
        .AppendIndented(nameof(Units), Units)
        .ToString();

    #endregion
    #region Data

    public string Name { get;  }
    public IHexLayer<Tile> Tiles { get; }
    public IHexLayer<Unit> Units { get; }
    
    #endregion
    #region Entity

    /// <summary>
    /// The unique identity of the entity.
    /// </summary>
    public ulong Identity { get; }

    /// <summary>
    /// The owner of the entity.
    /// </summary>
    public IController? Owner { get; }
    
    #endregion
    #region Variant

    /// <summary>
    /// The key this asset should be mapped to.
    /// </summary>
    public string Key => Name;

    #endregion
}
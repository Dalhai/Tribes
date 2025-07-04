using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public interface IEntity : IIdentifiable
{
    /// <summary>
    /// The owner of the entity.
    /// </summary>
    IController? Owner { get; }
    
    /// <summary>
    /// The location of the entity.
    /// </summary>
    AxialCoordinate Location { get; set; }
}

public interface IEntity<out TConfiguration> : IEntity
{
    /// <summary>
    /// The configuration of the entity.
    /// </summary>
    TConfiguration Configuration { get; }
}
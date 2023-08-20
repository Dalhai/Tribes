using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities;

public interface IEntity : IIdentifiable
{
    /// <summary>
    /// The owner of the entity.
    /// </summary>
    IController? Owner { get; }
}

public interface IEntity<out TConfiguration> : IEntity
{
    /// <summary>
    /// The configuration of the entity.
    /// </summary>
    TConfiguration Configuration { get; }
}
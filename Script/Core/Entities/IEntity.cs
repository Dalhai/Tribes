using TribesOfDust.Core.Controllers;

namespace TribesOfDust.Core.Entities;

public interface IEntity
{
    /// <summary>
    /// The unique identity of the entity.
    /// </summary>
    ulong Identity { get; }
    
    /// <summary>
    /// The owner of the entity.
    /// </summary>
    IController? Owner { get; }
}

public static class Identities
{
    /// <summary>
    /// Gets the next globally unique identity.
    /// </summary>
    public static ulong GetNextIdentity() => _lastIdentity++;
    private static ulong _lastIdentity;
}
namespace TribesOfDust.Core.Entities;

public interface IEntity
{
    /// <summary>
    /// Gets a unique identity for the entity.
    /// </summary>
    ulong Identity { get; }
}

public static class Identities
{
    /// <summary>
    /// Gets the next globally unique identity.
    /// </summary>
    public static ulong GetNextIdentity() => _lastIdentity++;
    private static ulong _lastIdentity = 0;
}
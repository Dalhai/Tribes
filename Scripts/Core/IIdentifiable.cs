namespace TribesOfDust.Core;

public interface IIdentifiable
{
    /// <summary>
    /// The unique identity of the entity.
    /// </summary>
    ulong Identity { get; }
}

public static class Identities
{
    /// <summary>
    /// Gets the next globally unique identity.
    /// </summary>
    public static ulong GetNextIdentity() => _lastIdentity++;
    private static ulong _lastIdentity;
}

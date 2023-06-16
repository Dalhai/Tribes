using static System.Diagnostics.Debug;

namespace TribesOfDust.Core;

public abstract class Entity
{
    #region Constructors

    protected Entity()
    {
        // This should never ever happen, you're insane if you get here.
        Assert(_nextId < ulong.MaxValue);
        
        // Claim the next id and increase it.
        // Not thread safe, might need fixing.
        Id = _nextId++;
    }
    
    #endregion
    #region Id
    
    /// <summary>
    /// A guaranteed to be unique identifier for the entity.
    /// </summary>
    ///
    /// <remarks>
    /// The identity is guaranteed to be global for all entities up to the point that anyone actually
    /// manages to create more entities than a ulong can hold. If you manage to do that, you deserve
    /// a medal and this needs to be fixed. For the moment, this should be fine.
    /// </remarks>
    public readonly ulong Id;
    private static ulong _nextId;
    
    #endregion
}
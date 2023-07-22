using System;

namespace TribesOfDust.Hex.Placement;


public interface ILocatable
{
    public record MovementArgs
    (
        AxialCoordinate Old, 
        AxialCoordinate New
    );
    
    public event EventHandler<MovementArgs> Moving;
    public event EventHandler<MovementArgs> Moved;
    
    /// <summary>
    /// Gets the coordinates of the locatable entity.
    /// </summary>
    AxialCoordinate Coordinates { get; }
}
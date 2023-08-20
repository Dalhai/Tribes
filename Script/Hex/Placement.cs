using System;

namespace TribesOfDust.Hex;

public interface IPlacement
{
    /// <summary>
    /// The coordinate of the entity on a layer or null, if the
    /// entity has not been placed on any associated layer.
    /// </summary>
    AxialCoordinate? Location { get; }
}

/// <summary>
/// A shareable placement with a shared coordinate.
/// </summary>
///
/// <remarks>
/// The purpose of this class is to share an axial coordinate between
/// multiple places, allowing changing the position of an entity without
/// needing to know anything else about the entity.
///
/// This also allows us to restrict assignment to the coordinate to
/// code that has access to the raw placement instead of the placement
/// interface.
/// </remarks>
public class Placement : IPlacement
{
    public delegate void CallbackDelegate(
        AxialCoordinate? oldCoordinate, 
        AxialCoordinate? newCoordinate
    );

    public CallbackDelegate? Callback
    {
        get => _callback;
        set => _callback = value;
    }
    
    public AxialCoordinate? Location
    {
        get => _location;
        set
        {
            var oldCoordinate = _location;
            var newCoordinate = value;
            
            _location = value;

            Callback?.Invoke(oldCoordinate, newCoordinate);
        }
    }

    private CallbackDelegate? _callback;
    private AxialCoordinate? _location;
}
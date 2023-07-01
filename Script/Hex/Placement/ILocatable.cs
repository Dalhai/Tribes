namespace TribesOfDust.Hex;

public interface ILocatable
{
    /// <summary>
    /// Gets the coordinates of the locatable entity.
    /// </summary>
    AxialCoordinate Coordinates { get; }
}
namespace TribesOfDust.Hex.Placement;

public interface IMovable : ILocatable
{
    /// <summary>
    /// Gets or sets the coordinates of the movable entity.
    /// </summary>
    new AxialCoordinate Coordinates { get; set; }
}
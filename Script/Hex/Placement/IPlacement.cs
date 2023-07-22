using TribesOfDust.Core;
using TribesOfDust.Core.Entities;

namespace TribesOfDust.Hex.Placement;

public interface IPlacement<T> where T: IEntity
{
    Map Map { get; }
    T Entity { get;  }
    AxialCoordinate Coordinates { get; set; }
}
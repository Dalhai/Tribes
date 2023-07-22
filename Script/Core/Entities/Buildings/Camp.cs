using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities.Buildings;

public class Camp : Building
{
    public Camp(AxialCoordinate coordinates, BuildingClass @class, IController owner) 
        : base(coordinates, @class, owner)
    {
    }
}


using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities.Buildings;

public class Camp : Building
{
    public Camp(AxialCoordinate coordinates, BuildingConfiguration configuration, IController owner) 
        : base(coordinates, configuration, owner)
    {
    }
}


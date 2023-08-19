using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core.Entities.Buildings;

public class Camp : Building
{
    public Camp(IHexLayer<Building> buildings, AxialCoordinate coordinates, BuildingConfiguration configuration, IController owner) 
        : base(buildings, coordinates, configuration, owner)
    {
    }
}


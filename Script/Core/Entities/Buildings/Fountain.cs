using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities.Buildings;

public class Fountain : Building
{
    public Fountain(AxialCoordinate coordinates, BuildingConfiguration configuration) 
        : base(coordinates, configuration, null)
    {
    }
}


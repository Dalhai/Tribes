using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities.Buildings;

public class Fountain : Building
{
    public Fountain(AxialCoordinate location, BuildingConfiguration configuration) 
        : base(location, configuration, null)
    {
    }
}


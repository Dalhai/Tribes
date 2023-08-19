using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core.Entities.Buildings;

public class Fountain : Building
{
    public Fountain(IHexLayer<Building> buildings, AxialCoordinate location, BuildingConfiguration configuration) 
        : base(buildings, location, configuration, null)
    {
    }
}


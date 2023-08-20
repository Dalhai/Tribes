using TribesOfDust.Core.Controllers;

namespace TribesOfDust.Core.Entities.Buildings;

public class Camp : Building
{
    public Camp(BuildingConfiguration configuration, IController owner) 
        : base(configuration, owner)
    {
    }
}


using static System.Diagnostics.Debug;

using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core.Entities;

public abstract class Building : IEntity<BuildingConfiguration>
{
    protected Building(IHexLayer<Building> buildings, AxialCoordinate location, BuildingConfiguration configuration, IController? owner)
    {
        Configuration = configuration;
        
        Owner = owner;
        
        // Initialize the location within the layer
        
        _location = location;
        _buildings = buildings;

        if (_buildings.Get(Location) is null)
            _buildings.Add(this, Location);
        else Assert(false);
        
        // TODO (MM): What do we do when there is already something at that spot?
        // This seems like something we should handle, but possibly not in the constructor.
        // This seems like the perfect case for an exception though.
    }

    public ulong Identity { get; } = Identities.GetNextIdentity();
    public IController? Owner { get; }
    public BuildingConfiguration Configuration { get; }

    public AxialCoordinate Location
    {
        get => _location;
        set
        {
            if (_location == value)
                return;

            if (_buildings.Get(_location) is { } building && building.Identity == Identity)
                _buildings.Remove(_location);
            else Assert(false);
            
            _location = value;
            _buildings.Add(this, _location);
        }
    }

    private readonly IHexLayer<Building> _buildings;
    private AxialCoordinate _location;
}
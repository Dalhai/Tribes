using static System.Diagnostics.Debug;

using TribesOfDust.Core.Controllers;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Core.Entities;

public class Unit : IEntity<UnitConfiguration>
{
    public Unit(IHexLayer<Unit> units, AxialCoordinate location, UnitConfiguration configuration, IController owner)
    {
        Configuration = configuration;
        
        Owner = owner;
        
        // Initialize the location within the layer
        
        _location = location;
        _units = units;

        if (_units.Get(Location) is null)
            _units.Add(this, Location);
        else Assert(false);
        
        // TODO (MM): What do we do when there is already something at that spot?
        // This seems like something we should handle, but possibly not in the constructor.
        // This seems like the perfect case for an exception though.
    }

    public ulong Identity { get; } = Identities.GetNextIdentity();
    public IController? Owner { get; }
    public UnitConfiguration Configuration { get; }

    private AxialCoordinate _location;
    public AxialCoordinate Location
    {
        get => _location;
        set
        {
            if (_location == value)
                return;

            if (_units.Get(_location) is { } unit && unit.Identity == Identity)
                _units.Remove(_location);
            else Assert(false);

            _location = value;
            _units.Add(this, _location);
        }
    }

    private readonly IHexLayer<Unit> _units;

    public double Health { get; set; } = 10;
    public double MaxHealth { get; } = 10;
    public double Water { get; set; } = 10;
    public double MaxWater { get; } = 10;
    public double Speed { get; } = 5;
}
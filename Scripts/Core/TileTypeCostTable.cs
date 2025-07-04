using Godot;

namespace TribesOfDust.Core;

public partial class TileTypeCostTable : Resource
{
    [Export] public double Tundra = 1.0;
    [Export] public double Rocks  = 1.0;
    [Export] public double Dunes  = 1.0;
    [Export] public double Canyon = 1.0;
}
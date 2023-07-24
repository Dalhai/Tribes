using Godot;
using TribesOfDust.Hex;

namespace TribesOfDust.Core.Entities.Buildings;

public class Fountain : Building
{
    public Fountain(AxialCoordinate coordinates, BuildingClass @class) 
        : base(coordinates, @class, null)
    {
        Sprite.Modulate = Colors.Aqua;
    }
}


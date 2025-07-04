using Godot;

namespace  TribesOfDust.Core.Controllers;

public interface IController
{
    #region Default

    string ToString() => $"Name: {Name}";

    #endregion
    #region Queries
    
    string Name { get; }
    Color Color { get; }
    Controller Type { get;}
    
    #endregion
}
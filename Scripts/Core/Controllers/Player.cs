using Godot;

namespace TribesOfDust.Core.Controllers
{
    public class Player : IController
    {
        #region Constructor
        
        public Player(string name, Color color)
        
        {
            Name = name;
            Color = color;
        }
        
        #endregion
        #region Overrides

        public override string ToString() => $"Player: {Name}";

        #endregion
        #region Queries

        public string Name { get; }
        public Color Color { get; }
        public Controller Type => Controller.Player;
        
        #endregion
    }
}
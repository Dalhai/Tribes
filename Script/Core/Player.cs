using Godot;

namespace TribesOfDust.Core
{
    public class Player
    {
        public Player(string name, Color color)
        {
            Name = name;
            Color = color;
        }

        #region Overrides

        public override string ToString() => $"Name: {Name}";

        #endregion

        public string Name { get; }
        public Color Color { get; }
    }
}
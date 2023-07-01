using Godot;

using System.Runtime.Serialization;

using TribesOfDust.Utils;

namespace TribesOfDust.Core
{
    public class Player : IVariant<string>
    {
        public Player(string name, long id, Color? color = null)
        {
            Name = name;
            Id = id;

            Color = color ?? Colors.White;
        }

        #region Overrides

        public override string ToString() => $"Name: {Name}";

        #endregion

        /// <summary>
        /// The name of the player.
        /// 
        /// Not guaranteed to be unique, so make sure not to use it as a unique
        /// identity for the player. Instead, refer to <see cref="Player.Id"/>
        /// for an identity that is unique across network instances as well.
        /// </summary>
        public string Name { get; }
        string IVariant<string>.Key => Name;

        /// <summary>
        /// Guaranteed unique player number within the same game instance.
        /// </summary>
        public long Id { get; init; } = 0;

        /// <summary>
        /// The color of the player.
        /// 
        /// Note that the active overlay decides what color is applied to different
        /// entities, but overlays can make use of this color to highlight things by
        /// player color.
        /// </summary>
        public Color Color { get; init; }
    }
}
using Godot;

namespace TribesOfDust.Core.Entities;

public interface IConfiguration
{
    /// <summary>
    /// The texture representing the entity on the map.
    /// </summary>
    Texture2D? Texture { get; }
}
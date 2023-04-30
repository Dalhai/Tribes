using Godot;

namespace TribesOfDust.Core;
    
public interface IContext<T> where T: Node
{
    /// <summary>
    /// The root node of the context.
    /// </summary>
    T Root { get; }
}
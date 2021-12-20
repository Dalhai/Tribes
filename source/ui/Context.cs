using Godot;

using TribesOfDust.UI;
using TribesOfDust.UI.Navigation;

namespace TribesOfDust.UI 
{
    public class Context 
    {   
        /// <summary>
        /// Try to get the current context based on the godot viewport.
        /// </summary>
        /// <param name="start">The child node in the tree to start from.</param>
        /// <returns>The root node context, if found, null otherwise.</returns>
        public static Context? Get(Node start) => Root.Get(start)?.Context;

        public Context(Root root) 
        {
            Root = root;
            Navigator = new(root);
        }

        /// <summary>
        /// Provides information about the route that was taken to get to
        /// the current state of the root node.
        /// </summary>
        public RouteArgs? Route { get; set; }

        /// <summary>
        /// The root navigator handling all scenes attached to the provided root node.
        /// </summary>
        public Navigator Navigator { get; init; }

        public readonly Root Root;
    }
}
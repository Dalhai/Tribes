using Godot;

namespace TribesOfDust.UI
{
    public class Root : Node 
    {
        /// <summary>
        /// Try to get the current root node based on the godot viewport.
        /// </summary>
        /// <param name="start">The child node in the tree to start from.</param>
        /// <returns>The root node, if found, null otherwise.</returns>
        public static Root? Get(Node start) => start.GetTree().Root.GetNodeOrNull<Root>("Root");

        public Root() 
        { 
            Context = new(this);

            // Setup named routes
            Context.Navigator.Route("main", context => (Node2D) GD.Load<PackedScene>("res://scenes/ui/pages/main_page.tscn").Instance());
            Context.Navigator.Route("editor", context => (Node2D) GD.Load<PackedScene>("res://scenes/ui/pages/editor_page.tscn").Instance());
        }

        /// <summary>
        /// The context provides ui elements with a simple way to exchange state easily.
        /// The context is always provided by parent elements or predecessors in the routing sequence.
        /// </summary>
        public Context Context { get; private init; }

        /// <summary>
        /// Gets the current scene node if there is any.
        /// </summary>
        public Node2D? Scene => GetNodeOrNull<Node2D>("Scene");

        public override void _EnterTree()
        {
            if (Scene is not null)
                GetTree().CurrentScene = Scene;

            base._EnterTree();
        }

    }
}
using Godot;
using TribesOfDust.UI.Navigation;

namespace TribesOfDust.Core
{
	[Tool]
	public partial class Root : Node, INavigatable<Node2D>
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
			Context.Navigator.Route("main", context => (Node2D)GD.Load<PackedScene>("res://scenes/ui/pages/main_page.tscn").Instantiate());
			Context.Navigator.Route("editor", context => (Node2D)GD.Load<PackedScene>("res://scenes/ui/pages/editor_page.tscn").Instantiate());
		}

		/// <summary>
		/// The context provides ui elements with a simple way to exchange state easily.
		/// The context is always provided by parent elements or predecessors in the routing sequence.
		/// </summary>
		public Context Context { get; private init; }

		/// <summary>
		/// Gets the current scene node if there is any.
		/// </summary>
		public Node2D? Scene
		{
			get => GetNodeOrNull<Node2D>("Scene");
			set
			{
				Node2D? scene = Scene;

				// Check that the new scene is not equal to the existing scene.
				// Otherwise, we might accidentally free the new scene.

				if (scene == value)
					return;

				// Properly free the old scene node.

				if (scene is not null)
				{
					RemoveChild(scene);
					scene.QueueFree();
				}

				// Properly attach the new scene node.

				if (value is not null)
				{
					value.Name = "Scene";
					AddChild(value);
				}
			}
		}

		/// <summary>
		/// Navigates to the page represented by the specified node.
		/// </summary>
		/// <param name="target">The node containing the next scene.</param>
		/// <param name="route">The arguments passed along the route.</param>
		/// <returns>True, if the scene was properly loaded, false otherwise.</returns>
		bool INavigatable<Node2D>.NavigateTo(Node2D target, RouteArgs route)
		{
			Scene = target;

			// Scene could potentially be null after assignment.
			// Check is necessary to signal navigation success and failure.
			return Scene is not null;
		}
	}
}

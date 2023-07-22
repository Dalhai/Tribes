using Godot;
using TribesOfDust.Core.Modes;
using TribesOfDust.UI.Navigation;

namespace TribesOfDust.Core;

public partial class Context : Node, 
	ISingleton<Context>,  
	INavigatable<Node2D>
{
	#region Constructors
	
	public Context()
	{
		// Setup named routes
		Navigator = new(this);
		Navigator.Route("main",   _ => (Node2D)GD.Load<PackedScene>("res://Interface/Page/main_page.tscn").Instantiate());
		Navigator.Route("play",   _ => (Node2D)GD.Load<PackedScene>("res://Interface/Page/game_page.tscn").Instantiate());
		Navigator.Route("editor", _ => (Node2D)GD.Load<PackedScene>("res://Interface/Page/editor_page.tscn").Instantiate());
	}
		
	#endregion
	#region Singleton
	
	public static Context Instance { get; private set; } = null!;
	public EditorMode? Editor => EditorMode.Instance;
	public MainMode? Main => MainMode.Instance;
	
	public override void _Ready() { Instance = this; } 
	
	#endregion
	#region Navigation
	
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
	/// Root Navigator
	/// 
	/// Use this to change between different scenes. For example, to get to the
	/// main menu from anywhere, or to get to the editor or the game menu from 
	/// anywhere as well. Note that some pages expect certain states to be set
	/// on the route args, read up on the individual page route args in the 
	/// different pages.
	/// </summary>
	public Navigator Navigator { get; }

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
	
	#endregion
}
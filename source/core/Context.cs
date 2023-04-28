using Godot;
using System.Text;
using TribesOfDust.UI.Navigation;
using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core ;

public partial class Context : RefCounted, IContext<Root>
{
    /// <summary>
    /// Try to get the current context based on the godot viewport.
    /// </summary>
    /// <param name="start">The child node in the tree to start from.</param>
    /// <returns>The root node context, if found, null otherwise.</returns>
    public static Context? GetRootContext(Node start) => Root.Get(start)?.Context;

    public Context(Root root) 
    {
        Root = root;

        // Initialize Subcontexts

        Game = new(this);
        Navigator = new(this, root);
    }

    #region Overrides

    public override string ToString() => new StringBuilder()
        .AppendIndented(nameof(Navigator), Navigator)
        .AppendIndented(nameof(Game), Game)
        .ToString();

    #endregion

    /// <summary>
    /// Root Navigator
    /// 
    /// Use this to change between different scenes. For example, to get to the
    /// main menu from anywhere, or to get to the editor or the game menu from 
    /// anywhere as well. Note that some pages expect certain states to be set
    /// on the route args, read up on the individual page route args in the 
    /// different pages.
    /// </summary>
    public Navigator<Node2D> Navigator { get; init; }

    /// <summary>
    /// Game Context
    /// 
    /// Contains information about the current game, including entities, state,
    /// game mode (editor or played) and others. If you need to know about what's
    /// going on on the map, this is the entity to check. It's a facade for everything
    /// that's going on and provides most of the information you need.
    /// </summary>
    public Game Game { get; init; }

    /// <summary>
    /// The root node of the context.
    /// </summary>
    public Root Root { get; init; }

}
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace TribesOfDust.UI.Navigation
{
    public class Navigator : Godot.Object
    {
        public Navigator(Root root) => Root = root;

        /// <summary>
        /// The root node to which nodes created through routes will be attached.
        /// The root node is also used as the parent for all manually created node trees.
        /// </summary>
        public readonly Root Root;
        public readonly Dictionary<string, Route> Routes = new();

        public void Route(string route, Func<Context, Node2D> createTarget)
        {
            if (Routes.ContainsKey(route))
                throw Error.CantAdd($"{nameof(route)}: {route}", this);

            Routes.Add(route, new(route, createTarget));
        }

        public bool GoTo(string route, object? args = null)
        {
            if (!Routes.ContainsKey(route) || Root is null)
                return false;

            var sceneNode = Root.GetNodeOrNull<Node2D>("Scene");
            if (sceneNode is not null)
                Root.RemoveChild(sceneNode);

            var selectedRoute = Routes[route];
            var selectedRouteArgs = new RouteArgs() 
            {
                Name = selectedRoute.Name,
                Source = selectedRoute.Source,
                Arguments = args,
            };

            return GoToCommon(selectedRoute.CreateTarget, selectedRouteArgs);
        }

        public bool GoTo(Func<Context, Node2D> createTarget, object? args = null)
        {
            if (Root is null)
                return false;

            var sceneNode = Root.GetNodeOrNull<Node2D>("Scene");
            if (sceneNode is not null)
                Root.RemoveChild(sceneNode);

            var selectedRouteArgs = new RouteArgs() 
            {
                Name = string.Empty,
                Arguments = args,
            };

            return GoToCommon(createTarget, selectedRouteArgs);
        }

        private bool GoToCommon(Func<Context, Node2D> createTarget, RouteArgs args)
        {
            if (Root is not null)
            {
                Root.Context.Route = args;

                var selectedTarget = createTarget.Invoke(Root.Context);
                selectedTarget.Name = "Scene";

                Root.AddChild(selectedTarget, true);
                Root.GetTree().CurrentScene = selectedTarget;

                return true;
            }

            return false;
        }
    }
}
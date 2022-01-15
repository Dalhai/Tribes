using Godot;

using System;
using System.Collections.Generic;

using TribesOfDust.Core;

namespace TribesOfDust.UI.Navigation
{
    public class Navigator : Godot.Object
    {
        public Navigator(Context context) => Context = context;

        /// <summary>
        /// The context this navigator belongs to.
        /// The context can be used to navigate the context tree.
        /// </summary>
        public readonly Context Context;

        /// <summary>
        /// The routes stored in this navigator.
        /// 
        /// This dictionary of routes tells the navigator which routes are accessible and
        /// how their corresponding root nodes are supposed to be constructed.
        /// </summary>
        public readonly Dictionary<string, Route> Routes = new();

        /// <summary>
        /// Adds a new route to the navigator.
        /// </summary>
        /// 
        /// <param name="route">The name under which the route can be found.</param>
        /// <param name="createTarget">The factory function used to create the root node.</param>
        public void Route(string route, Func<Context, Node2D> createTarget)
        {
            if (Routes.ContainsKey(route))
                throw Error.CantAdd($"{nameof(route)}: {route}", this);

            Routes.Add(route, new(route, createTarget));
        }

        /// <summary>
        /// Goes to the specified route and updates the context route args.
        /// </summary>
        /// 
        /// <param name="route">The route to take.</param>
        /// <param name="args">The arguments to pass along.</param>
        /// 
        /// <returns>True, if the route was successfuly taken, false otherwise.</returns>
        public bool GoTo(string route, object? args = null)
        {
            if (!Routes.ContainsKey(route))
                return false;

            var selectedRoute = Routes[route];
            var selectedRouteArgs = new RouteArgs() 
            {
                Name = selectedRoute.Name,
                Source = selectedRoute.Source,
                Arguments = args,
            };

            return GoToCommon(selectedRoute.CreateTarget, selectedRouteArgs);
        }

        /// <summary>
        /// Goes to a new scene without an associated route and updates the context route args.
        /// </summary>
        /// 
        /// <param name="createTarget">The factory function for the new scene.</param>
        /// <param name="args">The arguments to pass along.</param>
        /// 
        /// <returns>True, if the scene change was successful, false otherwise.</returns>
        public bool GoTo(Func<Context, Node2D> createTarget, object? args = null)
        {
            var selectedRouteArgs = new RouteArgs() 
            {
                Name = string.Empty,
                Arguments = args,
            };

            return GoToCommon(createTarget, selectedRouteArgs);
        }

        private bool GoToCommon(Func<Context, Node2D> createTarget, RouteArgs args)
        {
            Context.Route = args;
            Context.Root.Scene = createTarget.Invoke(Context);

            // We signal success if a new scene was properly created and attached to the root node.
            // If the scene wasn't attached, something must have gone wrong as we should never be
            // in a situation where there is not scene node.

            return Context.Root.Scene != null;
        }
    }
}
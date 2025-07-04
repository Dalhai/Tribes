using System;
using System.Collections.Generic;
using Godot;
using TribesOfDust.Core;

namespace TribesOfDust.UI.Navigation
{
    public class Navigator
    {
        public Navigator(Context context) { _context = context; }

        /// <summary>
        /// Adds a new route to the navigator.
        /// </summary>
        /// 
        /// <param name="route">The name under which the route can be found.</param>
        /// <param name="createTarget">The factory function used to create the root node.</param>
        public void Route(string route, Func<Context, Node2D> createTarget)
        {
            if (_routes.ContainsKey(route))
                throw Error.CantAdd($"{nameof(route)}: {route}", this);

            _routes.Add(route, new(route, createTarget));
        }

        /// <summary>
        /// Goes to the last navigated route.
        /// </summary>
        /// 
        /// <param name="args">The arguments to pass along.</param>
        /// 
        /// <returns>True, if the last route was loaded successfully, false otherwise.</returns>
        public bool Pop(object? args = null)
        {
            if (_routeHistory.Count == 0)
                return false;

            var latest = _routeHistory.Pop();
            var name = latest.Name;

            return name is not null && GoTo(name, args ?? latest.Arguments);
        }

        /// <summary>
        /// Goes to the specified route.
        /// </summary>
        /// 
        /// <param name="route">The route to take.</param>
        /// <param name="args">The arguments to pass along.</param>
        /// 
        /// <returns>True, if the route was successfully taken, false otherwise.</returns>
        public bool GoTo(string route, object? args = null)
        {
            if (!_routes.ContainsKey(route))
                return false;

            var selectedRoute = _routes[route];
            var selectedRouteArgs = new RouteArgs
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
            var selectedRouteArgs = new RouteArgs
            {
                Name = string.Empty,
                Arguments = args,
            };

            return GoToCommon(createTarget, selectedRouteArgs);
        }

        private bool GoToCommon(Func<Context, Node2D> createTarget, RouteArgs args)
        {
            var target = createTarget.Invoke(_context);

            // Push the route args to the history to enable pushing and popping
            // various routes without knowing the parent.
            _routeHistory.Push(args);

            // Pass the created target as well as the selected route arguments
            // to the navigated element and let it handle success and failure
            // reporting.
            return (_context as INavigatable<Node2D>).NavigateTo(target, args);
        }


        private readonly Dictionary<string, Route<Node2D>> _routes = new();
        private readonly Stack<RouteArgs> _routeHistory = new();
        
        private readonly Context _context;
    }
}
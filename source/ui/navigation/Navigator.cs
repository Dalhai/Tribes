using Godot;

using System;
using System.Collections.Generic;

using TribesOfDust.Core;

namespace TribesOfDust.UI.Navigation
{
    public class Navigator<TTarget>
    {
        public Navigator(Context context, INavigatable<TTarget> navigated)
        { 
            _navigated = navigated;
            _context = context;
        }

        /// <summary>
        /// The routes stored in this navigator.
        /// 
        /// This dictionary of routes tells the navigator which routes are accessible and
        /// how their corresponding root nodes are supposed to be constructed.
        /// </summary>
        public readonly Dictionary<string, Route<TTarget>> Routes = new();
        
        /// <summary>
        /// The routes taken so far using this navigator.
        /// </summary>
        public readonly Stack<RouteArgs> History = new();

        /// <summary>
        /// Adds a new route to the navigator.
        /// </summary>
        /// 
        /// <param name="route">The name under which the route can be found.</param>
        /// <param name="createTarget">The factory function used to create the root node.</param>
        public void Route(string route, Func<Context, TTarget> createTarget)
        {
            if (Routes.ContainsKey(route))
                throw Error.CantAdd($"{nameof(route)}: {route}", this);

            Routes.Add(route, new(route, createTarget));
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
            if (History.Count == 0)
                return false;

            var latest = History.Pop();
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
        public bool GoTo(Func<Context, TTarget> createTarget, object? args = null)
        {
            var selectedRouteArgs = new RouteArgs() 
            {
                Name = string.Empty,
                Arguments = args,
            };

            return GoToCommon(createTarget, selectedRouteArgs);
        }

        private bool GoToCommon(Func<Context, TTarget> createTarget, RouteArgs args)
        {
            var target = createTarget.Invoke(_context);

            // Push the route args to the history to enable pushing and popping
            // various routes without knowing the parent.
            History.Push(args);

            // Pass the created target as well as the selected route arguments
            // to the navigated element and let it handle success and failure
            // reporting.
            return _navigated.NavigateTo(target, args);
        }

        private readonly INavigatable<TTarget> _navigated;
        private readonly Context _context;
    }
}
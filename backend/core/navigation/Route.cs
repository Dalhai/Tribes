using Godot;
using System;
using TribesOfDust.Core;

namespace TribesOfDust.UI.Navigation
{
    public partial class Route<TTarget>
    {
        public Route(string name, Func<Context, TTarget> createTarget)
        {
            Name = name;
            CreateTarget = createTarget;
        }

        public string Name { get; }

        public object? Source { get; init; } 
        public object? Target { get; init; }

        public Func<Context, TTarget> CreateTarget { get; init; }
    }

    public partial class RouteArgs
    {
        public string? Name { get; init; }
        public object? Source { get; init; }
        public object? Arguments { get; init; }
    }
}
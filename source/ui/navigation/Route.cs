using Godot;
using System;
using TribesOfDust.Core;

namespace TribesOfDust.UI.Navigation
{
    public class Route
    {
        public Route(string name, Func<Context, Node2D> createTarget)
        {
            Name = name;
            CreateTarget = createTarget;
        }

        public string Name { get; }

        public object? Source { get; init; } 
        public object? Target { get; init; }

        public Func<Context, Node2D> CreateTarget { get; init; }
    }

    public class RouteArgs
    {
        public string? Name { get; init; }
        public object? Source { get; init; }
        public object? Arguments { get; init; }
    }
}
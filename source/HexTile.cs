using Godot;
using System;

public class HexTile : Sprite
{
    /// <summary>
    /// Cube coordinates based index.
    /// </summary>
    /// <param name="X">Left to right</param>
    /// <param name="Y">Bottom right to top left</param>
    /// <param name="Z">Top right to bottom left</param>
    public record HexIndex(int X, int Y, int Z);

    public HexIndex Index { get; init; }
}
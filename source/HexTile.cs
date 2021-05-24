using Godot;
using System;

public class HexTile : Sprite
{
    public record HexIndex(int Row, int Column);

    public HexIndex Index { get; init; }
}
using System;
using System.Drawing.Drawing2D;
using Godot;

namespace TribesOfDust.Hex.Storage
{
    public class TileStorageEventArgs<T> : EventArgs
    {
        public TileStorageEventArgs(AxialCoordinate coordinates, T item)
        {
            Coordinates = coordinates;
            Item = item;
        }

        public AxialCoordinate Coordinates { get; init; }
        public T Item { get; init; }
    }
}
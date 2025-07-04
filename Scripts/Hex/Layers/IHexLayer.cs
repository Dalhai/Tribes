namespace TribesOfDust.Hex.Layers;

public interface IHexLayer<T> : IHexLayerView<T> 
{
    #region Add

    /// <summary>
    /// Tries to add the item at the specified coordinates.
    /// </summary>
    /// <param name="location">The location to add the item at.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>True, if the item was added, false otherwise.</returns>
    bool TryAdd(AxialCoordinate location, T item);

    #endregion
    #region Remove

    /// <summary>
    /// Tries to remove the item at the specified coordinates.
    /// </summary>
    /// <param name="location">The location of the item to remove.</param>
    /// <returns>True, if the item was removed, false otherwise.</returns>
    bool TryRemove(AxialCoordinate location);
    void Clear();

    #endregion
}
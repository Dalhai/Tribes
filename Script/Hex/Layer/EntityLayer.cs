using System.Collections.Generic;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex.Storage;

namespace TribesOfDust.Hex.Layer;

public class EntityLayer<T> : HexLayer<T> where T: IEntity
{
    #region Add

    public override bool Add(T item, AxialCoordinate coordinates)
    {
        if (Contains(coordinates))
            return false;

        OnAdding(item, coordinates);
        Items.Add(coordinates, item);
        _coords.Add(item.Identity, coordinates);
        OnAdded(item, coordinates);

        return true;
    }
    
    #endregion
    #region Remove

    public override bool Remove(AxialCoordinate coordinates)
    {
        if (!Contains(coordinates))
            return false;

        var item = Get(coordinates);
        if (item is not null)
        {
            OnRemoving(item, coordinates);
            Items.Remove(coordinates);
            _coords.Remove(item.Identity);
            OnRemoved(item, coordinates);
        }

        return true;
    }
    
    #endregion
    #region Get

    public override bool Contains(T item) => _coords.ContainsKey(item.Identity);
    public override AxialCoordinate? GetCoordinates(T item) => _coords.ContainsKey(item.Identity) ? _coords[item.Identity] : null;

    #endregion

    private readonly Dictionary<ulong, AxialCoordinate> _coords = new();
}

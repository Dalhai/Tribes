using System.Collections.Generic;
using TribesOfDust.Core.Entities;
using TribesOfDust.Hex;
using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Gen;

public interface IHexLayerGenerator<T> where T: IEntity
{
    /// <summary>
    /// Fills the layer according to the generator specification.
    /// </summary>
    /// 
    /// <param name="layer">The layer to be filled.</param>
    /// <param name="placements">The newly created placements.</param>
    /// 
    /// <returns>True, if the layer was filled.</returns>
    /// <returns>False, if the layer could not be filled.</returns>
    bool Generate(IHexLayer<T> layer, IDictionary<ulong, Placement> placements);
}
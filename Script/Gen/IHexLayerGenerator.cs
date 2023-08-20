using TribesOfDust.Hex.Layers;

namespace TribesOfDust.Gen;

public interface IHexLayerGenerator<T>
{
    /// <summary>
    /// Fills the layer according to the generator specification.
    /// </summary>
    /// 
    /// <param name="layer">The layer to be filled.</param>
    /// 
    /// <returns>True, if the layer was filled.</returns>
    /// <returns>False, if the layer could not be filled.</returns>
    bool Generate(IHexLayer<T> layer);
}
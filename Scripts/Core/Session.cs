namespace TribesOfDust.Core
{
    public class Session
    {
        public Session(MapContext mapContext) => MapContext = mapContext;

        /// <summary>
        /// The game these repositories belong to.
        /// The game can be used to walk the context tree up.
        /// </summary>
        public readonly MapContext MapContext;
    }
}
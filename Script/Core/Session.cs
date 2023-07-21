using System.Linq;
using System.Collections.Generic;

namespace TribesOfDust.Core
{
    public partial class Session
    {
        public Session(MapContext mapContext) => MapContext = mapContext;

        /// <summary>
        /// The game these repositories belong to.
        /// The game can be used to walk the context tree up.
        /// </summary>
        public readonly MapContext MapContext;
    }
}
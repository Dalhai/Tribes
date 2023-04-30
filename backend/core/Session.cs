using System.Linq;
using System.Collections.Generic;

namespace TribesOfDust.Core
{
    public partial class Session
    {
        public Session(EditorContext editorContext) => EditorContext = editorContext;

        /// <summary>
        /// The game these repositories belong to.
        /// The game can be used to walk the context tree up.
        /// </summary>
        public readonly EditorContext EditorContext;
    }
}
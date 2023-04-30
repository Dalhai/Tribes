using System;
using System.Text;

using TribesOfDust.Utils.Extensions;

namespace TribesOfDust.Core
{
    public class EditorContext
    {
        public EditorContext(Context context)
        {
            Context = context;

            // Initialize sub contexts.
            Repositories = new();
            Level        = new(Repositories);
            Display      = new(Level.Tiles);
        }

        #region Overrides

        public override string ToString() => new StringBuilder()
            .AppendIndented(nameof(Repositories), Repositories)
            .AppendIndented(nameof(Level), Level)
            .AppendIndented(nameof(Display), Display)
            .ToString();

        #endregion

        /// <summary>
        /// The context this navigator belongs to.
        /// The context can be used to navigate the context tree.
        /// </summary>
        public Context Context { get; }

        /// <summary>
        /// The repositories used for this game.
        /// 
        /// Repositories automatically load assets for you based on how you access them.
        /// Repositories are also used to preload some often used assets.
        /// </summary>
        public readonly Repositories Repositories;

        /// <summary>
        /// The currently loaded level.
        /// 
        /// Contains information about all entities and tiles in the level.
        /// Contains information about health, stats and other properties of units.
        /// Contains information about fountains, baseas, ruins and effects.
        /// 
        /// In general, anything that is happening on a map is in some capacity represented here.
        /// Note that although everything on the map is represented here, how it is displayed
        /// is handled separately in the display layer.
        /// </summary>
        public readonly Level Level;

        /// <summary>
        /// All graphical elements of the current level.
        /// 
        /// Provides access to overlays, sprites, rendering settings and many more.
        /// If you need to add anything purely visual, such as overlays and on-map
        /// displays that are strictly local, this is the context to access.
        /// </summary>
        public readonly Display Display;
    }
}
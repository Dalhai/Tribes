using Godot;

using static System.Diagnostics.Debug;
using System.Collections.Generic;
using System.Linq;

using TribesOfDust.Hex;
using TribesOfDust.Hex.Storage;

namespace TribesOfDust.UI
{
    public class EditorMenu : Control 
    {
        public override void _EnterTree()
        {
            _tundraCount = GetNode<LabelValueItem>(TundraCountItemPath);
            _rocksCount = GetNode<LabelValueItem>(RocksCountItemPath);
            _dunesCount = GetNode<LabelValueItem>(DunesCountItemPath);
            _canyonCount = GetNode<LabelValueItem>(CanyonCountItemPath);
            _availableCount = GetNode<LabelValueItem>(AvailableCountItemPath);

            Assert(_tundraCount is not null);
            Assert(_rocksCount is not null);
            Assert(_dunesCount is not null);
            Assert(_canyonCount is not null);
            Assert(_availableCount is not null);

            base._Ready();
        }

        public void UpdateCounts(ITileStorageView<Tile> tiles,  IDictionary<TileType, int> tilePool)
        {
            int tundraCount = tilePool.TryGetValue(TileType.Tundra, out tundraCount) ? tundraCount : 0;
            int rocksCount = tilePool.TryGetValue(TileType.Rocks, out rocksCount) ? rocksCount : 0;
            int dunesCount = tilePool.TryGetValue(TileType.Dunes, out dunesCount) ? dunesCount : 0;
            int canyonCount = tilePool.TryGetValue(TileType.Canyon, out canyonCount) ? canyonCount : 0;

            // Get the number of available (open) tiles on the map currently.

            int availableCount = tiles.Count(tile => tile.Value.Key == TileType.Open);

            // Update all label counts on the editor menu.

            _tundraCount.Value = tundraCount.ToString();
            _rocksCount.Value = rocksCount.ToString();
            _dunesCount.Value = dunesCount.ToString();
            _canyonCount.Value = canyonCount.ToString();
            _availableCount.Value = availableCount.ToString();
        }

        private LabelValueItem _tundraCount = null!;
        private LabelValueItem _rocksCount = null!;
        private LabelValueItem _dunesCount = null!;
        private LabelValueItem _canyonCount = null!;
        private LabelValueItem _availableCount = null!;

        private const string TundraCountItemPath = "List/TundraCount";
        private const string RocksCountItemPath = "List/RocksCount";
        private const string DunesCountItemPath = "List/DunesCount";
        private const string CanyonCountItemPath = "List/CanyonCount";
        private const string AvailableCountItemPath = "List/AvailableCount";
    }
}
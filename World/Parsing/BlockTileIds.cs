using System.Collections.Generic;
using TeamUno.Mario.Entities.Items;

namespace TeamUno.Mario.World
{
    /// <summary>
    /// All knowledge about block-sheet tile IDs: maps a raw ID to the canonical reference ID (port of MapSystem::getReferenceBlockID) and exposes the tile-kind predicates derived from it.
    /// </summary>
    internal static class BlockTileIds
    {
        private static readonly IReadOnlyDictionary<int, int> IrregularBlockReferences =
            new Dictionary<int, int>
            {
                { 592, 337 }, { 593, 101 }, { 594, 150 }, { 595, 151 }, { 596, 339 },
                { 640, 385 }, { 641, 149 }, { 642, 198 }, { 643, 199 }, { 644, 338 },
                { 645, 201 }, { 646, 202 }, { 647, 203 }, { 648, 56 }, { 649, 57 },
                { 650, 58 }, { 652, 59 }, { 653, 60 }, { 654, 61 }, { 655, 62 },
                { 688, 244 }, { 689, 245 }, { 690, 246 }, { 691, 247 }, { 692, 248 },
                { 693, 249 }, { 694, 250 }, { 695, 251 }, { 696, 252 }, { 697, 105 },
                { 701, 108 }, { 702, 109 }, { 703, 110 },
                { 736, 292 }, { 737, 293 }, { 738, 294 }, { 739, 295 }, { 740, 296 },
                { 741, 297 }, { 742, 298 }, { 743, 299 }, { 744, 300 }, { 745, 153 },
                { 746, 301 }, { 747, 302 }, { 748, 303 }, { 749, 156 }, { 750, 157 },
                { 751, 158 },
                { 832, 337 }, { 833, 101 }, { 834, 150 }, { 835, 151 }, { 836, 339 },
                { 880, 385 }, { 881, 149 }, { 882, 198 }, { 883, 199 }, { 884, 338 },
                { 885, 201 }, { 886, 202 }, { 887, 203 }, { 888, 56 }, { 889, 57 },
                { 890, 58 }, { 892, 59 }, { 893, 60 }, { 894, 61 }, { 895, 62 },
                { 928, 244 }, { 929, 245 }, { 930, 246 }, { 931, 247 }, { 932, 248 },
                { 933, 249 }, { 934, 250 }, { 935, 251 }, { 936, 252 }, { 937, 105 },
                { 941, 108 }, { 942, 109 }, { 943, 110 },
                { 976, 292 }, { 977, 293 }, { 978, 294 }, { 979, 295 }, { 980, 296 },
                { 981, 297 }, { 982, 298 }, { 983, 299 }, { 984, 300 }, { 985, 153 },
                { 986, 301 }, { 987, 302 }, { 988, 303 }, { 989, 156 }, { 990, 157 },
                { 991, 158 }
            };

        internal static int GetReferenceBlockId(int tileId)
        {
            if (tileId < 0)
            {
                return tileId;
            }

            int referenceId;
            if (IrregularBlockReferences.TryGetValue(tileId, out referenceId))
            {
                return referenceId;
            }

            int coordinateX = tileId % 48;
            int coordinateY = tileId / 48;
            int originalY = coordinateY;

            if (coordinateY > 10 && coordinateX < 32)
            {
                coordinateY -= 11;
            }

            if (coordinateX > 15 && coordinateX < 32)
            {
                coordinateX -= 16;
            }
            else if (coordinateX >= 32 && originalY < 10)
            {
                coordinateX -= 32;
            }

            return coordinateY * 48 + coordinateX;
        }

        internal static int? GetUsedTileId(int tileId)
        {
            int referenceTileId = GetReferenceBlockId(tileId);
            if (referenceTileId != 192 && referenceTileId != 289 && referenceTileId != 290
                && referenceTileId != 720)
            {
                return null;
            }

            int entityCoordinateX = tileId % 48;
            int entityCoordinateY = tileId / 48;
            int referenceCoordinateX = 196 % 48;
            int referenceCoordinateY = 196 / 48;
            if (entityCoordinateY > 10)
            {
                referenceCoordinateY += 11;
            }

            if (entityCoordinateX > 15 && entityCoordinateX < 32)
            {
                referenceCoordinateX += 16;
            }
            else if (entityCoordinateX >= 32)
            {
                referenceCoordinateX += 32;
            }

            return referenceCoordinateY * 48 + referenceCoordinateX;
        }

        internal static bool IsBrickTile(int tileId)
        {
            int referenceTileId = GetReferenceBlockId(tileId);
            return referenceTileId == 289 || referenceTileId == 290;
        }

        internal static bool IsPipeTile(int tileId)
        {
            int referenceTileId = GetReferenceBlockId(tileId);
            return referenceTileId == 150 || referenceTileId == 151
                || referenceTileId == 198 || referenceTileId == 199
                || referenceTileId >= 244 && referenceTileId <= 247
                || referenceTileId >= 292 && referenceTileId <= 295;
        }

        internal static bool IsFlagPoleTile(int tileId)
        {
            int referenceTileId = GetReferenceBlockId(tileId);
            return referenceTileId == 101 || referenceTileId == 149;
        }

        internal static bool IsSpringPartTile(int referenceTileId)
        {
            return referenceTileId == 347 || referenceTileId == 348
                || referenceTileId == 394 || referenceTileId == 395 || referenceTileId == 396;
        }

        internal static bool IsNonCollidableTile(int tileId)
        {
            int referenceTileId = GetReferenceBlockId(tileId);
            return referenceTileId == 101 || referenceTileId == 149 || referenceTileId == 152
                || IsSpringPartTile(referenceTileId)
                || referenceTileId == 761 || referenceTileId == 762
                || referenceTileId == 809 || referenceTileId == 810 || referenceTileId == 811
                || referenceTileId == 858 || referenceTileId == 859;
        }

        internal static ItemType? GetMapItemType(int tileId)
        {
            return GetReferenceBlockId(tileId) == 144
                ? ItemType.FloatingCoin
                : (ItemType?)null;
        }
    }
}

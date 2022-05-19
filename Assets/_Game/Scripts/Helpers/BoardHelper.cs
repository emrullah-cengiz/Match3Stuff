using Assets.Scripts.Models;
using Assets.Scripts.Other;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Actors
{
    public static class BoardHelper
    {
        public static BoardSettings BoardSettings => GameManager.Instance.BoardSettings;

        public static Vector2 BoardTopLeftCorner => new Vector2(-BoardSettings.HorizontalTileCount / 2 * BoardSettings.TileWidth,
                                                                 BoardSettings.VerticalTileCount / 2 * BoardSettings.TileWidth);

        public static Vector2 GetDropPositionByMatrixPosition(int x, int y) =>
            BoardTopLeftCorner + new Vector2(BoardSettings.DropMargin + x * BoardSettings.TileWidth
                                          - (BoardSettings.HorizontalTileCount % 2 == 0 ? 0 : BoardSettings.TileWidth / 2),

                                             -BoardSettings.DropMargin - y * BoardSettings.TileWidth
                                           + (BoardSettings.VerticalTileCount % 2 == 0 ? 0 : BoardSettings.TileWidth / 2));

        public static Drop[,] SetupDropMatrix() =>
             new Drop[BoardSettings.HorizontalTileCount,
                      BoardSettings.VerticalTileCount];

        public static Drop GetDropPrefabByType(DropType dropType) =>
            BoardSettings.DropPrefabs.FirstOrDefault(x => x.DropType == dropType);

    }
}
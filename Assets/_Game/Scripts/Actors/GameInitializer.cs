using Assets.Scripts.Actors;
using Assets.Scripts.Models;
using Assets.Scripts.Other;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public Transform dropsTransform;

    public void Awake()
    {
        GameManager.Instance.DropMatrix = BoardHelper.SetupDropMatrix();

        FillTheBoard();
    }

    public void FillTheBoard()
    {
        for (int y = 0; y < BoardHelper.BoardSettings.VerticalTileCount; y++)
        {
            for (int x = 0; x < BoardHelper.BoardSettings.HorizontalTileCount; x++)
            {
                DropType dropType = GetRandomSuitableDropTypeByPoint(new Vector2Int(x, y));

                var dropPrefab = BoardHelper.GetDropPrefabByType(dropType);

                Vector2 pos = BoardHelper.GetDropPositionByMatrixPosition(x, y);

                var drop = Instantiate(dropPrefab, pos, Quaternion.identity, dropsTransform);

                GameManager.Instance.DropMatrix[x, y] = drop;
                drop.positionOnMatrix = new Vector2Int(x, y);
            }
        }
    }

    private DropType GetRandomSuitableDropTypeByPoint(Vector2Int refPoint)
    {
        List<Drop> matchedHorDrops;
        List<Drop> matchedVerDrops;
        List<DropType> excludesList = new();
        DropType dropType;

        do
        {
            matchedHorDrops = new();
            matchedVerDrops = new();

            dropType = Utility.GetRandomEnumValue<DropType>(excludesList);
            excludesList.Add(dropType);

            GameManager.Instance.CheckMatchByDirection(new CheckMatchModel(dropType, refPoint, Vector2Int.left), ref matchedHorDrops);
            GameManager.Instance.CheckMatchByDirection(new CheckMatchModel(dropType, refPoint, Vector2Int.down), ref matchedVerDrops);
            //Vector2Int.up has the opposite effect on matrix system

            if (matchedHorDrops.Count < 2)
                matchedHorDrops.Clear();
            if (matchedVerDrops.Count < 2)
                matchedVerDrops.Clear();

        } while (matchedHorDrops.Count + matchedVerDrops.Count > 0);

        return dropType;
    }
}

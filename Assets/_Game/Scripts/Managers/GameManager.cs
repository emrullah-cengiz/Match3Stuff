using Assets.Scripts.Actors;
using Assets.Scripts.Infrastructure;
using Assets.Scripts.Models;
using Assets.Scripts.Other;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : Singleton<GameManager>
{
    public BoardSettings BoardSettings;
    public Drop[,] DropMatrix;

    public void Swipe(Drop drop, Vector2Int direction)
    {
        var neighborToChange = GetNeighborDropByDirection(drop.positionOnMatrix, direction);
        bool matchFounded;

        if (neighborToChange == null)
            return;

        ControllerManager.Instance.CanSwipe = false;

        //Check Match For Both of Drops
        List<Drop> allMatchedDrops = new();
        matchFounded = GetMathchedPointsOfSwipedDrops(drop, neighborToChange, direction, ref allMatchedDrops);

        #region Slide drops effect

        drop.spriteRenderer.sortingOrder += 2;
        drop.Swipe(neighborToChange.transform.position,
                   callbackAction: () =>
                   {
                       if (!matchFounded)
                           //Slide back the drop
                           drop.Swipe(neighborToChange.transform.position,
                                      callbackAction: () =>
                                      {
                                          SwipeCallback(drop);
                                          ControllerManager.Instance.CanSwipe = true;
                                      });
                       else
                       {
                           SwipeCallback(drop);

                           ProcessAfterMatchOperations(allMatchedDrops);
                       }
                   });

        neighborToChange.Swipe(drop.transform.position,
            callbackAction: () =>
            {
                //Switch back drop
                if (!matchFounded)
                    neighborToChange.Swipe(drop.transform.position);
            });

        if (matchFounded)
        {
            DropMatrix[drop.positionOnMatrix.x, drop.positionOnMatrix.y] = neighborToChange;
            DropMatrix[neighborToChange.positionOnMatrix.x, neighborToChange.positionOnMatrix.y] = drop;

            Vector2Int tempPosition = drop.positionOnMatrix;

            drop.positionOnMatrix = neighborToChange.positionOnMatrix;
            neighborToChange.positionOnMatrix = tempPosition;
        }

        #endregion
    }

    public void SwipeCallback(Drop drop)
    {
        drop.spriteRenderer.sortingOrder -= 2;
    }

    public void BlowUpDrops(List<Drop> matchedDrops)
    {
        //Blow up matched drops
        foreach (var drop in matchedDrops)
        {
            DropMatrix[drop.positionOnMatrix.x, drop.positionOnMatrix.y] = null;
            drop.BlowUp();

            DropPoolingManager.Instance.EnqueueDrop(drop);
        }
    }

    public void RePositionFallableDropsOnMatrixAndSpawnNewDrops(List<Drop> matchedDrops, out List<Drop> dropsToFall)
    {
        dropsToFall = new();

        var groupedEmptyTilesByColumn = matchedDrops.Select(d => d.positionOnMatrix)
                                                    .OrderBy(d => d.x)
                                                    .ThenBy(d => d.y)
                                                    .GroupBy(d => d.x);

        foreach (var emptyTilesByColumn in groupedEmptyTilesByColumn)
        {
            Vector2Int firstEmptyTile = emptyTilesByColumn.FirstOrDefault();
            int lastEmptyTileY = emptyTilesByColumn.LastOrDefault().y;

            //It's gives drop from bottom to top on column
            var availableFallableDropsEnumerator = GetDropsByMatrixColumn(x: firstEmptyTile.x,
                                                                          maxY: lastEmptyTileY);

            bool allAvaileableDropsAreRelocated = false;

            for (int y = lastEmptyTileY; y >= 0; y--)
            {
                //Shift Drops to bottom empty tiles
                Drop dropToFall = null;
                if (!allAvaileableDropsAreRelocated)
                {
                    dropToFall = availableFallableDropsEnumerator.GetNext();

                    allAvaileableDropsAreRelocated = dropToFall == null;

                    //set null to old position of drop on matrix
                    if (dropToFall != null)
                        DropMatrix[dropToFall.positionOnMatrix.x, dropToFall.positionOnMatrix.y] = null;

                }

                if (allAvaileableDropsAreRelocated)
                    if (BoardSettings.SpawnerColumnIndexes.Contains(firstEmptyTile.x))
                    {
                        //Spawn the new drops
                        dropToFall = DropPoolingManager.Instance.GetDropByType(Utility.GetRandomEnumValue<DropType>());

                        //Set spawn position to out of board by queue
                        dropToFall.transform.position = BoardHelper.GetDropPositionByMatrixPosition(firstEmptyTile.x, y - emptyTilesByColumn.Count());
                        dropToFall.gameObject.SetActive(true);
                    }
                    else
                        break;

                //set new position data to drop 
                dropToFall.positionOnMatrix = new Vector2Int(firstEmptyTile.x, y);

                //set drop to own new position on matrix
                DropMatrix[firstEmptyTile.x, y] = dropToFall;

                dropsToFall.Add(dropToFall);
            }
        }
    }

    private IEnumerator<Drop> GetDropsByMatrixColumn(int x, int? minY = null, int? maxY = null)
    {
        for (int i = BoardSettings.VerticalTileCount - 1; i >= 0; i--)
            if ((!minY.HasValue || i >= minY) && (!maxY.HasValue || i <= maxY))
                if (DropMatrix[x, i] != null)
                    yield return DropMatrix[x, i];
    }

    #region Match Checking

    public bool GetMathchedPointsOfSwipedDrops(Drop firstDrop, Drop secondDrop, Vector2Int direction, ref List<Drop> allMatchedDrops)
    {
        List<Drop> matchedFirstDrops = new();

        GetMathchedPointsOnMatrix(new CheckMatchModel(firstDrop.DropType, secondDrop.positionOnMatrix), -direction, ref matchedFirstDrops);

        List<Drop> matchedSecondDrops = new();

        GetMathchedPointsOnMatrix(new CheckMatchModel(secondDrop.DropType, firstDrop.positionOnMatrix), direction, ref matchedSecondDrops);

        if (matchedFirstDrops.Any())
            matchedFirstDrops.Add(firstDrop);
        if (matchedSecondDrops.Any())
            matchedSecondDrops.Add(secondDrop);

        allMatchedDrops.AddRange(matchedFirstDrops);
        allMatchedDrops.AddRange(matchedSecondDrops);

        return allMatchedDrops.Any();
    }

    /// <summary>
    /// Use CheckMatchModel.Direction for opposite operation on this function 
    /// </summary>
    /// <param name="checkMatchModel">Use CheckMatchModel.Direction for opposite operation on this function</param>
    /// <param name="matchedDrops"></param>
    /// <returns></returns>
    public bool GetMathchedPointsOnMatrix(CheckMatchModel checkMatchModel, Vector2Int dontLookDirection, ref List<Drop> matchedDrops)
    {
        List<Drop> matchedHorizontalDrops = new();

        checkMatchModel.Direction = Vector2Int.left;
        if (dontLookDirection != checkMatchModel.Direction)
            CheckMatchByDirection(checkMatchModel, ref matchedHorizontalDrops);

        checkMatchModel.Direction = Vector2Int.right;
        if (dontLookDirection != checkMatchModel.Direction)
            CheckMatchByDirection(checkMatchModel, ref matchedHorizontalDrops);

        if (matchedHorizontalDrops.Count < 2)
            matchedHorizontalDrops.Clear();

        List<Drop> matchedVerticalDrops = new();

        checkMatchModel.Direction = Vector2Int.up;
        if (dontLookDirection != checkMatchModel.Direction)
            CheckMatchByDirection(checkMatchModel, ref matchedVerticalDrops);

        checkMatchModel.Direction = Vector2Int.down;
        if (dontLookDirection != checkMatchModel.Direction)
            CheckMatchByDirection(checkMatchModel, ref matchedVerticalDrops);

        if (matchedVerticalDrops.Count < 2)
            matchedVerticalDrops.Clear();

        matchedDrops.AddRange(matchedHorizontalDrops);
        matchedDrops.AddRange(matchedVerticalDrops);

        return matchedHorizontalDrops.Count + matchedVerticalDrops.Count > 0;
    }

    public bool CheckMatchByDirection(CheckMatchModel checkMatchModel, ref List<Drop> matchedDrops)
    {
        List<Drop> matchedDropsOnCurrentDirection = new();
        var positionToLook = checkMatchModel.StartPointToLook;

        do
        {
            Drop neighbor = GetNeighborDropByDirection(positionToLook, checkMatchModel.Direction);

            if (neighbor == null)
                break;

            positionToLook = neighbor.positionOnMatrix;

            if (neighbor.DropType == checkMatchModel.DropType)
            {
                if (matchedDropsOnCurrentDirection.Count > 20)
                    print(positionToLook + " - " + checkMatchModel.Direction + " - " + matchedDropsOnCurrentDirection.Count);

                matchedDropsOnCurrentDirection.Add(neighbor);
            }
            else
                break;

        } while (true);

        matchedDrops.AddRange(matchedDropsOnCurrentDirection);

        return matchedDropsOnCurrentDirection.Any();

    }

    public void CheckAndAutoBlowUpDropsByReferenceDrops(List<Drop> drops)
    {
        List<Drop> matchedDrops = new();

        foreach (var drop in drops)
        {
            if (GetMathchedPointsOnMatrix(new CheckMatchModel(drop.DropType, drop.positionOnMatrix), Vector2Int.zero, ref matchedDrops))
                matchedDrops.Add(drop);
        }

        if (!matchedDrops.Any())
        {
            ControllerManager.Instance.CanSwipe = true;
            return;
        }

        matchedDrops = matchedDrops.Distinct().ToList();

        ProcessAfterMatchOperations(matchedDrops);
    }

    public void ProcessAfterMatchOperations(List<Drop> matchedDrops)
    {
        BlowUpDrops(matchedDrops);

        RePositionFallableDropsOnMatrixAndSpawnNewDrops(matchedDrops, out List<Drop> dropsToFall);

        StartCoroutine(Utility.WaitForSeconds(BoardSettings.DropBlowUpDuration,
            (object[] args) =>
            {
                dropsToFall.ForEach(x => x.DropDownToYourOwnMatrixPosition());

                StartCoroutine(Utility.WaitForSeconds(BoardSettings.DropFallDuration,
                    (object[] args) => CheckAndAutoBlowUpDropsByReferenceDrops(dropsToFall)));

            }));
    }

    #endregion

    #region Helpers

    private Drop GetNeighborDropByDirection(Vector2Int positionOnMatrix, Vector2Int direction)
    {
        var pos = positionOnMatrix + direction;
        pos.Clamp(Vector2Int.zero, new Vector2Int(BoardSettings.HorizontalTileCount - 1,
                                                  BoardSettings.VerticalTileCount - 1));

        return pos == positionOnMatrix ? null : DropMatrix[pos.x, pos.y];
    }

    #endregion

}


using Assets.Scripts.Other;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    [Serializable]
    [CreateAssetMenu(fileName = "BoardSettings", menuName = "Settings/BoardSettings", order = 0)]
    public class BoardSettings : ScriptableObject
    {
        [Header("Board")]
        public int HorizontalTileCount = 8;
        public int VerticalTileCount = 8;
        public float TileWidth;
        public List<int> SpawnerColumnIndexes;

        [Header("Drop")]
        public float DropMargin = .5f;

        public float DropSwipeDuration = 1f;
        public Ease DropSwipeEase = Ease.Linear;

        public float DropFallDelay = .06f;
        public float DropFallDuration = 1f;
        public Ease DropFallEase = Ease.Linear;

        public float DropBlowUpDuration = .5f;

        public List<Drop> DropPrefabs = new();

        public Vector2 GetDropScaleValue() => DropPrefabs.FirstOrDefault().spriteRenderer.transform.localScale;
    }
}
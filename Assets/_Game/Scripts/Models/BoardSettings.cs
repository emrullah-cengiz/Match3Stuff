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
        [field: SerializeField] public int HorizontalTileCount = 8;
        [field: SerializeField] public int VerticalTileCount = 8;
        [field: HideInInspector] public float TileWidth;
        [field: SerializeField] public List<int> SpawnerColumnIndexes;

        [Header("Drop")]
        [field: SerializeField] public float DropMargin = .5f;
        [field: SerializeField] public float DropSwitchDuration = 1f;
        [field: SerializeField] public float DropBlowUpDuration = .5f;
        [field: SerializeField] public float DropFallDelay = .06f;
        [field: SerializeField] public float DropFallDuration = 1f;
        [field: SerializeField] public Ease DropFallEase = Ease.Linear;

        [field: SerializeField] public List<Drop> DropPrefabs = new();

        public Vector2 GetDropScaleValue() => DropPrefabs.FirstOrDefault().transform.localScale;
    }
}
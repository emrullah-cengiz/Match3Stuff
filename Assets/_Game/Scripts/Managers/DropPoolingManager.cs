using Assets.Scripts.Actors;
using Assets.Scripts.Other;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Infrastructure
{
    [Serializable]
    public struct DropPool
    {
        public DropPool(DropType dropType)
        {
            DropType = dropType;
            DropQueue = new Queue<Drop>();
        }

        public DropType DropType { get; set; }
        public Queue<Drop> DropQueue { get; set; }

        public void AddDrop()
        {
            var drop = DropPoolingManager.Instance.InstantiateDropByType(DropType);
            drop.gameObject.SetActive(false);

            DropQueue.Enqueue(drop);
        }
    }

    public class DropPoolingManager : Singleton<DropPoolingManager>
    {
        public Transform dropsTransform;

        [SerializeField] private int AutoInstantiateDropCountOnStartPerType = 10;
        [SerializeField] private int AutoInstantiateDropCountWhenOutOfDropPerType = 5;

        [HideInInspector] public List<DropPool> DropPools;

        public override void _Awake()
        {
            SetupPools();
        }

        public void EnqueueDrop(Drop drop)
        {
            var pool = GetDropPoolByType(drop.DropType);

            pool.DropQueue.Enqueue(drop);
        }

        public Drop GetDropByType(DropType dropType)
        {
            var pool = GetDropPoolByType(dropType);

            if (pool.DropQueue.Count == 0)
                for (int i = 0; i < AutoInstantiateDropCountWhenOutOfDropPerType; i++)
                    pool.AddDrop();

            return pool.DropQueue.Dequeue();
        }

        private DropPool GetDropPoolByType(DropType dropType) =>
                DropPools.FirstOrDefault(x => x.DropType == dropType);

        private void SetupPools()
        {
            foreach (var dropType in Utility.GetEnumValues<DropType>())
            {
                DropPool pool = new DropPool(dropType);

                if (AutoInstantiateDropCountOnStartPerType > 0)
                    for (int i = 0; i < AutoInstantiateDropCountOnStartPerType; i++)
                        pool.AddDrop();

                DropPools.Add(pool);
            }
        }

        public Drop InstantiateDropByType(DropType dropType)
        {
            var drop = Instantiate(BoardHelper.GetDropPrefabByType(dropType), dropsTransform);
            drop.positionOnMatrix = new Vector2Int(-1, -1);
            return drop;
        }


    }
}
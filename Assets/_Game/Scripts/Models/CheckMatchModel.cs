using Assets.Scripts.Other;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class CheckMatchModel
    {
        public CheckMatchModel(DropType dropType,
                               Vector2Int startPointToLook)
        {
            DropType = dropType;
            StartPointToLook = startPointToLook;
        }

        public CheckMatchModel(DropType dropType,
                               Vector2Int startPointToLook,
                               Vector2Int direction)
        {
            DropType = dropType;
            StartPointToLook = startPointToLook;
            Direction = direction;
        }

        public DropType DropType { get; set; }
        public Vector2Int StartPointToLook { get; set; }
        public Vector2Int Direction { get; set; }

    }
}
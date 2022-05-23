using Assets.Scripts.Infrastructure;
using Assets.Scripts.Models;
using Assets.Scripts.Other;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerManager : Singleton<ControllerManager>
{
    public bool CanSwipe = true;

    [SerializeField] private LayerMask DropLayerMask;
    [SerializeField] private float minSwipeMagnitude = 8f;

    private Drop HoldedDrop;
    private Vector2 holdStartPos;

    private void Update()
    {
        if (CanSwipe)
            SwipeHandler();
    }

    private void SwipeHandler()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HoldedDrop = GetTouchedDrop();

            if (HoldedDrop == null)
                return;

            holdStartPos = GetTouchPosition();
        }
        else if (Input.GetMouseButton(0))
        {
            if (HoldedDrop == null)
                return;

            Vector2 currentHoldPos = GetTouchPosition();

            Vector2 delta = currentHoldPos - holdStartPos;

            if (delta.magnitude > minSwipeMagnitude)
            {
                GameManager.Instance.Swipe(HoldedDrop, GetDirectionByDelta(delta));
                HoldedDrop = null;
            }
            else
                return;
        }
        else if (Input.GetMouseButtonUp(0))
            HoldedDrop = null;
    }


    #region Helpers
    private Vector2Int GetDirectionByDelta(Vector2 delta)
    {
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            if (delta.x < 0)
                return Vector2Int.left;
            else
                return Vector2Int.right;
        }
        else
        {
            if (delta.y < 0)
                return Vector2Int.up;
            else
                return Vector2Int.down;
        }
    }
    private Drop GetTouchedDrop()
    {
        Vector3 touchPos = GetTouchPosition();

        touchPos = Camera.main.ScreenToWorldPoint(touchPos);

        var collider = Physics2D.OverlapPoint(touchPos, DropLayerMask);

        return collider?.GetComponent<Drop>();
    }
    private Vector3 GetTouchPosition()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.OSXEditor)
            return Input.mousePosition;
        else
            return Input.GetTouch(0).position;
    }
    #endregion
}


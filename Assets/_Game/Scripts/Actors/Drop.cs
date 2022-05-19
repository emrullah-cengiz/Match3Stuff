using Assets.Scripts.Actors;
using Assets.Scripts.Models;
using Assets.Scripts.Other;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public DropType DropType;
    [HideInInspector] public Vector2Int positionOnMatrix;

    private Sequence tweenSequence;
  
    #region Animations / Effects

    public void Swipe(Vector2 targetPos, Action callbackAction = null)
    {
        tweenSequence?.Kill(true);

        tweenSequence = DOTween.Sequence();

        tweenSequence.Join(transform.DOMove(targetPos, BoardHelper.BoardSettings.DropSwitchDuration));
        tweenSequence.Join(transform.DOPunchScale(Vector3.one * .2f, BoardHelper.BoardSettings.DropSwitchDuration, 0, 0));

        tweenSequence.OnComplete(() =>
        {
            callbackAction?.Invoke();
        });

        tweenSequence.Play();

    }

    public void DropDownToYourOwnMatrixPosition(float delayToBegin = 0)
    {
        tweenSequence?.Kill(true);

        tweenSequence = DOTween.Sequence();

        tweenSequence.AppendInterval(delayToBegin);

        //scale
        tweenSequence.Append(spriteRenderer.transform.DOScaleY(GameManager.Instance.dropScaleValue.y + .15f, BoardHelper.BoardSettings.DropFallDuration));
        tweenSequence.Join(spriteRenderer.transform.DOScaleX(GameManager.Instance.dropScaleValue.x - .1f, BoardHelper.BoardSettings.DropFallDuration));

        //fall
        tweenSequence.Join(transform.DOMove(BoardHelper.GetDropPositionByMatrixPosition(positionOnMatrix.x, positionOnMatrix.y),
                                            BoardHelper.BoardSettings.DropFallDuration)
                                    .SetEase(BoardHelper.BoardSettings.DropFallEase));

        //scale back
        tweenSequence.Append(spriteRenderer.transform.DOScaleY(GameManager.Instance.dropScaleValue.y, BoardHelper.BoardSettings.DropFallDuration / 3));
        tweenSequence.Append(spriteRenderer.transform.DOScaleX(GameManager.Instance.dropScaleValue.x, BoardHelper.BoardSettings.DropFallDuration / 3));

        tweenSequence.Play();
    }
    public void BlowUp()
    {
        tweenSequence?.Kill(true);

        tweenSequence = DOTween.Sequence();

        tweenSequence.Join(spriteRenderer.transform.DOPunchScale(Vector3.one * .4f, BoardHelper.BoardSettings.DropBlowUpDuration / 3 * 2, 5, 5));
        tweenSequence.Append(spriteRenderer.transform.DOScale(Vector3.zero, BoardHelper.BoardSettings.DropBlowUpDuration / 3));

        tweenSequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            spriteRenderer.transform.localScale = Vector3.one;
        });

        tweenSequence.Play();
    }

    #endregion
}

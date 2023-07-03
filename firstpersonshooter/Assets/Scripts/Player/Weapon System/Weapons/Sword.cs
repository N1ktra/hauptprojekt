using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Melee
{
    public override void swing(Action OnComplete)
    {
        float duration = 1 / attackSpeed;
        Sequence swingSequence = DOTween.Sequence();
        swingSequence.Append(transform.DOLocalRotate(new Vector3(0, 20, -90), duration * 1 / 2));
        swingSequence.Join(transform.DOLocalMove(new Vector3(.25f, 0, .5f), duration * 1 / 2));
        swingSequence.Append(transform.DOLocalRotate(new Vector3(0, -90, -45), duration * 1 / 2));
        swingSequence.Join(transform.DOLocalMove(new Vector3(-1f, 0, .5f), duration * 1 / 2));
        swingSequence.OnComplete(() => { OnComplete?.Invoke(); });
    }
}

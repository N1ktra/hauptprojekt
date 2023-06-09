using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Weapon
{
    public float recoverySpeed = 5f;

    protected override void Start()
    {
        base.Start();
    }

    protected void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * recoverySpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * recoverySpeed);
    }

    public override void Attack(bool isFirstShot)
    {
        if (Time.time >= nextTimeToAttack)
        {
            swing();
            nextTimeToAttack = Time.time + (1f / attackSpeed);
        }
    }
    private bool swinging = false;
    private void swing()
    {
        //Camera cam = Camera.main;
        //Vector3 hitPos = cam.transform.position + cam.transform.forward * 2;

        float duration = 1/attackSpeed;

        swinging = true;
        Sequence swingSequence = DOTween.Sequence();
        swingSequence.Append(transform.DOLocalMove(new Vector3(.5f, .5f, 0), duration * 1 / 2));
        swingSequence.Join(transform.DOLocalRotate(new Vector3(-90, 0, 0), duration * 1 / 2));
        swingSequence.Append(transform.DOLocalMove(new Vector3(-.5f, 0, 2), duration * 1 / 2));
        swingSequence.Join(transform.DOLocalRotate(new Vector3(45, -30, 0), duration * 1 / 2));
        swingSequence.OnComplete(() => { swinging = false; });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (swinging)
        {
            var collisionPoint = other.ClosestPointOnBounds(transform.position);
            var collisionNormal = transform.position - collisionPoint;
            RaiseOnHitEvent(this, new AttackEventArgs(other.gameObject, collisionPoint, collisionNormal));
        }
    }


}

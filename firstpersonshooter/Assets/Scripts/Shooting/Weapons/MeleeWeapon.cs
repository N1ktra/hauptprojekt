using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MeleeWeapon : Weapon
{
    public override void Shoot(bool isFirstShot)
    {
        if (Time.time >= nextTimeToFire)
        {
            swing();
            nextTimeToFire = Time.time + (1f / firerate);
        }
    }
    private bool swinging = false;
    private void swing()
    {
        Camera cam = Camera.main;
        Vector3 hitPos = cam.transform.position + cam.transform.forward * 3;

        float duration = 1/firerate;
        swinging = true;
        Sequence swingSequence = DOTween.Sequence();
        swingSequence.Append(transform.DOLocalMove(new Vector3(.5f, .5f, 0), duration * 3 / 4));
        swingSequence.Join(transform.DOLocalRotate(new Vector3(-90, 0, 0), duration * 3 / 4));
        swingSequence.Append(transform.DOMove(hitPos, duration * 1 / 4));
        swingSequence.Join(transform.DOLocalRotate(Vector3.zero, duration * 1 / 4));
        swingSequence.OnComplete(() => { swinging = false; });
        swingSequence.SetEase(Ease.InCubic);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (swinging)
        {
            Debug.Log(other.gameObject.name);
            DealDamage(this, new ShootEventArgs(other.gameObject, other.transform.position, other.transform.forward));
        }
    }
}

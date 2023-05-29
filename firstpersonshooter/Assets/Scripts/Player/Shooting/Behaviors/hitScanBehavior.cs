using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun Behaviors/Hit Scan")]
public class hitScanBehavior : ShootBehavior
{
    public override void Shoot(Gun gun)
    {
        GameObject cam = Camera.main.gameObject;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            gun.RaiseOnHitEvent(this, new AttackEventArgs(hit.transform.gameObject, hit.point, hit.normal));
            CreateTrail(gun, hit.point);
        }
        else
            CreateTrail(gun, cam.transform.position + cam.transform.forward * 10f);
    }

    public void CreateTrail(Gun gun, Vector3 hitPoint)
    {
        if (gun.bulletTrail == null) return;
        float travelTime = gun.bulletTrail.time;
        TrailRenderer trail = Instantiate(gun.bulletTrail, gun.BulletSpawnPoint.position, Quaternion.identity);
        trail.transform.DOMove(hitPoint, travelTime).SetEase(Ease.InExpo);
    }
}

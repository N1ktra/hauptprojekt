using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitScanBehavior : MonoBehaviour, IGunBehavior
{
    public event ShootEventHandler OnHit;

    private GameObject cam;
    private void Start()
    {
        cam = Camera.main.gameObject;
    }

    public void Shoot(Gun gun)
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            OnHit?.Invoke(this, new ShootEventArgs(hit.transform.gameObject, hit.point, hit.normal));
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

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitScanBehavior : MonoBehaviour, IWeaponBehavior
{
    public event ShootEventHandler OnHit;

    private GameObject cam;
    private void Start()
    {
        cam = Camera.main.gameObject;
    }

    public void Shoot(Weapon weapon)
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            OnHit?.Invoke(this, new ShootEventArgs(hit.transform.gameObject, hit.point, hit.normal));
            CreateTrail(weapon, hit.point);
        }
        else
            CreateTrail(weapon, cam.transform.position + cam.transform.forward * 10f);
    }

    public void CreateTrail(Weapon weapon, Vector3 hitPoint)
    {
        float travelTime = weapon.bulletTrail.time;
        TrailRenderer trail = Instantiate(weapon.bulletTrail, weapon.BulletSpawnPoint.position, Quaternion.identity);
        trail.transform.DOMove(hitPoint, travelTime).SetEase(Ease.InExpo).OnComplete(() =>
        {
            Destroy(trail.gameObject);
        });
    }
}

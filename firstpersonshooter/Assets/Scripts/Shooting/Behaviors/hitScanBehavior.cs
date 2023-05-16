using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitScanBehavior : MonoBehaviour, IWeaponBehavior
{
    public event IWeaponBehavior.ShootEvent OnHit;

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
            Debug.Log(hit.transform.name);
        }
    }


}

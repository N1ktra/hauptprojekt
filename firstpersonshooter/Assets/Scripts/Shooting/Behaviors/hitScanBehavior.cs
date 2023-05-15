using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitScanBehavior : MonoBehaviour, IWeaponBehavior
{
    public event IWeaponBehavior.ShootEvent OnHit;

    public void Shoot(Weapon weapon)
    {
        GameObject cam = weapon.transform.parent.parent.gameObject;
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            OnHit?.Invoke(this, new ShootEventArgs(hit.transform.gameObject, hit.point, hit.normal));
            Debug.Log(hit.transform.name);
        }
    }


}

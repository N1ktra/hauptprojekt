using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hitScanBehavior : MonoBehaviour, IWeaponBehavior
{
    public void Shoot(Weapon weapon)
    {
        GameObject cam = weapon.transform.parent.parent.gameObject;
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            Debug.Log(hit.transform.name);
            
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.takeDamage(weapon.damage);
            }
        }
    }
}

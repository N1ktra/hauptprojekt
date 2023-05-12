using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public IWeaponBehavior weaponBehavior;
    public bool isAutomatic;
    public float firerate;
    public float damage;
    public ProjectileInfo projectile;

    private void Start()
    {
        weaponBehavior = GetComponent<IWeaponBehavior>();
    }

    private float nextTimeToFire = 0f;
    /// <summary>
    /// Schieﬂt (beachtet jedoch Feuerrate)
    /// </summary>
    public void Shoot()
    {
        if(Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (1f / firerate);
            weaponBehavior.Shoot(this);
        }
    }
}

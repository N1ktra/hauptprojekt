using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public IWeaponBehavior weaponBehavior;

    [Header("Attributes")]
    public bool isAutomatic;
    public float firerate;
    public float damage;

    [Header("Projectiles")]
    public Projectile projectile;
    public float projectileSpeed;

    [Header("VFX")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private void Start()
    {
        weaponBehavior = GetComponent<IWeaponBehavior>();
        weaponBehavior.OnHit += DealDamage;
        weaponBehavior.OnHit += PlayImpactEffect;
    }

    private float nextTimeToFire = 0f;
    /// <summary>
    /// Schie�t (beachtet jedoch Feuerrate)
    /// </summary>
    public void Shoot()
    {
        if(Time.time >= nextTimeToFire)
        {
            muzzleFlash.Play();
            nextTimeToFire = Time.time + (1f / firerate);
            weaponBehavior.Shoot(this);
        }
    }

    /// <summary>
    /// Deal Damage to a Target
    /// </summary>
    /// <param name="target">Has to be an enemy in order to deal Damage</param>
    public void DealDamage(object sender, ShootEventArgs e)
    {
        Enemy enemy = e.hitObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.takeDamage(damage);
        }
    }

    private void PlayImpactEffect(object sender, ShootEventArgs e)
    {
        GameObject impactGO = Instantiate(impactEffect, e.hitPosition, Quaternion.LookRotation(e.hitDirection));
        Destroy(impactGO, 1f);
    }
}

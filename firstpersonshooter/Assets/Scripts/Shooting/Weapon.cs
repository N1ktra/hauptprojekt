using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public IWeaponBehavior weaponBehavior;
    public bool isAutomatic;
    public float firerate;
    public float damage;
    public Projectile projectile;
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;

    private void Start()
    {
        weaponBehavior = GetComponent<IWeaponBehavior>();
        weaponBehavior.OnHit += DealDamage;
    }

    private float nextTimeToFire = 0f;
    /// <summary>
    /// Schieﬂt (beachtet jedoch Feuerrate)
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
    /// Instanziiert ein neues Projektil und setzt die Referenz auf diese Waffe
    /// </summary>
    /// <param name="pos">Position des Projektils</param>
    /// <param name="rot">Rotation des Projektils</param>
    /// <returns></returns>
    public GameObject InstantiateProjectile(Vector3 pos, Quaternion rot)
    {
        GameObject obj = Instantiate(projectile.gameObject, pos, rot);
        projectile = obj.GetComponent<Projectile>();
        projectile.weapon = this;
        return obj;
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
}

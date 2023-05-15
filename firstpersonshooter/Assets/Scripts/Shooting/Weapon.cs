using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

[RequireComponent(typeof(IWeaponBehavior))]
public abstract class Weapon : MonoBehaviour
{
    public IWeaponBehavior weaponBehavior;
    public bool isAutomatic;
    public float firerate;
    public float damage;
    [SerializeField] private Projectile projectile;
    [SerializeField] private ParticleSystem muzzleFlash;

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public IWeaponBehavior weaponBehavior;
    private CameraShake cameraShake;

    [Header("Attributes")]
    public bool isAutomatic;
    public float firerate;
    public float damage;

    [Header("Recoil")]
    [SerializeField] private float recoilAmount;
    [SerializeField] private float recoverySpeed;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    [Header("Projectiles")]
    public Projectile projectile;
    public float projectileSpeed;

    [Header("VFX")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private void Start()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        originalRotation = transform.rotation;
        weaponBehavior = GetComponent<IWeaponBehavior>();
        weaponBehavior.OnHit += DealDamage;
        weaponBehavior.OnHit += PlayImpactEffect;
    }

    private void Update()
    {
        RecoverFromRecoil();
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
            ApplyRecoilForce();
            cameraShake.Shake(.1f, .005f);
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

    private void ApplyRecoilForce()
    {
        transform.Rotate(new Vector3(0, 0, -recoilAmount * 15));
        transform.localPosition -= recoilAmount * Vector3.forward;
    }

    void RecoverFromRecoil()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * recoverySpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, originalRotation, Time.deltaTime * recoverySpeed);
    }
}

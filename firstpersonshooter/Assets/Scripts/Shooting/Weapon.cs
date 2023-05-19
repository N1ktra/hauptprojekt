using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public IWeaponBehavior weaponBehavior;
    private CameraFunctions cameraFunctions;

    [Header("Attributes")]
    public bool isAutomatic;
    public float firerate;
    public float damage;

    [Header("Recoil")]
    [SerializeField] private float recoilForce;
    [SerializeField] private float recoverySpeed;
    [SerializeField] private float maxRecoilAngle;
    [SerializeField] private float recoilAmount;
    private Quaternion originalWeaponRotation;
    private Vector3 originalWeaponPosition;

    [Header("Projectiles")]
    public Projectile projectile;
    public float projectileSpeed;

    [Header("VFX")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private void Start()
    {
        cameraFunctions = Camera.main.transform.parent.gameObject.GetComponent<CameraFunctions>();
        originalWeaponRotation = transform.localRotation;
        originalWeaponPosition = transform.localPosition;
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
        //Weapon recoil
        transform.RotateAround(transform.position, cameraFunctions.transform.right, -recoilForce * 30f);
        transform.localPosition -= recoilForce * Vector3.forward;

        //Camera recoil
        cameraFunctions.Shake(.1f, .001f);
        Vector3 cameraRot = cameraFunctions.getRotation();
        if (Mathf.Abs(cameraRot.x) < maxRecoilAngle)
            cameraFunctions.Rotate(new Vector3(-recoilAmount, 0, 0));
    }

    void RecoverFromRecoil()
    {
        //Weapon recoil
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalWeaponPosition, Time.deltaTime * recoverySpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, originalWeaponRotation, Time.deltaTime * recoverySpeed);

        //Camera recoil
        if (!Input.GetMouseButton(0))
            cameraFunctions.RotateTo(Quaternion.Slerp(cameraFunctions.transform.localRotation, Quaternion.identity, Time.deltaTime * recoverySpeed));
    }
}

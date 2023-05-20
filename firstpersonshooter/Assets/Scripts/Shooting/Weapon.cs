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
    public int maxAmmo;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float reloadTime;
    private bool isReloading;

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
        cameraFunctions = GetComponentInParent<CameraFunctions>();
        weaponBehavior = GetComponent<IWeaponBehavior>();
        originalWeaponRotation = transform.localRotation;
        originalWeaponPosition = transform.localPosition;

        weaponBehavior.OnHit += DealDamage;
        weaponBehavior.OnHit += PlayImpactEffect;
        currentAmmo = maxAmmo;
    }

    public void Update()
    {
        RecoverFromRecoil();
    }

    private float nextTimeToFire = 0f;
    /// <summary>
    /// Schieﬂt (beachtet jedoch Feuerrate)
    /// </summary>
    /// <param name="firstShot">auf true setzen, falls das der erste Schuss ist (bei automatik)</param>
    public void Shoot(bool firstShot)
    {
        if (isReloading) return;
        if(currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }
        if(Time.time >= nextTimeToFire)
        {
            if(firstShot)
                cameraFunctions.resetRotation();
            weaponBehavior.Shoot(this);
            currentAmmo--;
            nextTimeToFire = Time.time + (1f / firerate);
            muzzleFlash.Play();
            ApplyRecoilForce();
        }
    }

    public IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        currentAmmo = maxAmmo;
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
        transform.RotateAround(transform.position, cameraFunctions.transform.right, -recoilForce * 10f);
        transform.localPosition -= recoilForce * Vector3.forward;

        //Camera recoil
        cameraFunctions.ScreenShake(.1f, .001f);
        if (Mathf.Abs(cameraFunctions.getVerticalAngle()) < maxRecoilAngle)
            cameraFunctions.RotateBy(new Vector3(-recoilAmount, 0, 0), .5f);
    }

    private void RecoverFromRecoil()
    {
        //Weapon recoil
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalWeaponPosition, Time.deltaTime * recoverySpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, originalWeaponRotation, Time.deltaTime * recoverySpeed);

        //Camera recoil
        if (!Input.GetMouseButton(0) || isReloading)
        {
            cameraFunctions.StopRotating();
            cameraFunctions.transform.localRotation = Quaternion.Slerp(cameraFunctions.transform.localRotation, cameraFunctions.lastSavedRotation, Time.deltaTime * recoverySpeed);
        }
    }
}

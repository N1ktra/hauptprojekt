using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public IWeaponBehavior weaponBehavior;
    [SerializeField] private CameraMovement cameraMovement;

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

    [Header("Projectiles")]
    public Projectile projectile;
    public float projectileSpeed;

    [Header("VFX")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public void Reset()
    {
        cameraMovement = GetComponentInParent<CameraMovement>();
    }

    public void Start()
    {
        weaponBehavior = GetComponent<IWeaponBehavior>();

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
    /// <param name="isFirstShot">auf true setzen, falls das der erste Schuss ist (bei automatik)</param>
    public void Shoot(bool isFirstShot)
    {
        if (isReloading)
        {
            cameraMovement.resetRotation();
            return;
        }
        if(currentAmmo <= 0)
        {
            Reload();
            return;
        }
        if(Time.time >= nextTimeToFire)
        {
            if(isFirstShot)
                cameraMovement.resetRotation();
            weaponBehavior.Shoot(this);
            currentAmmo--;
            nextTimeToFire = Time.time + (1f / firerate);
            if(muzzleFlash != null) muzzleFlash.Play();
            ApplyRecoilForce();
        }
    }

    public void Reload()
    {
        Debug.Log("Reloading...");
        isReloading = true;
        transform.DOLocalRotate(new Vector3(45, 0, 0), reloadTime/2).SetEase(Ease.InOutElastic)
        .OnComplete(() =>
        {
            transform.DOLocalRotate(new Vector3(0, 0, 0), reloadTime/2).SetEase(Ease.InOutElastic).OnComplete(() =>
            {
                isReloading = false;
                currentAmmo = maxAmmo;
            });
        });
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
        if (impactEffect == null) return;
        GameObject impactGO = Instantiate(impactEffect, e.hitPosition, Quaternion.LookRotation(e.hitDirection));
        Destroy(impactGO, 1f);
    }

    private void ApplyRecoilForce()
    {
        //Weapon recoil
        transform.Rotate(-recoilForce * 10f, 0, 0);
        transform.localPosition -= recoilForce * Vector3.forward;

        //Camera recoil
        cameraMovement.ScreenShake(.1f, .001f);
        if (Mathf.Abs(cameraMovement.getVerticalAngle()) < maxRecoilAngle)
            cameraMovement.RotateBy(new Vector3(-recoilAmount, 0, 0), .5f);
    }

    private void RecoverFromRecoil()
    {
        //Weapon recoil
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * recoverySpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * recoverySpeed);

        //Camera recoil
        if (!Input.GetMouseButton(0) || isReloading)
        {
            cameraMovement.StopRotating();
            cameraMovement.transform.localRotation = Quaternion.Slerp(cameraMovement.transform.localRotation, cameraMovement.lastSavedRotation, Time.deltaTime * recoverySpeed);
        }
    }
}

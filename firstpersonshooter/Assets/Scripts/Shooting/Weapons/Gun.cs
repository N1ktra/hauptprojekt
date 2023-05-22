using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public IGunBehavior gunBehavior;

    [Header("Ammo")]
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
    public Transform BulletSpawnPoint;
    public Projectile projectile;
    public float projectileSpeed;

    [Header("VFX")]
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public TrailRenderer bulletTrail;

    public void Start()
    {
        muzzleFlash = Instantiate(muzzleFlash, BulletSpawnPoint.position, Quaternion.identity, transform);
        gunBehavior = GetComponent<IGunBehavior>();
        if (gunBehavior != null)
        {
            gunBehavior.OnHit += (object sender, ShootEventArgs e) => { DealDamage(e.hitObject); };
            gunBehavior.OnHit += PlayImpactEffect;
        }

        currentAmmo = maxAmmo;
    }

    public void Update()
    {
        RecoverFromRecoil();
    }

    public override void Attack(bool isFirstShot)
    {
        if (isReloading)
        {
            cameraMovement.resetRotation();
            return;
        }
        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }
        if (Time.time >= nextTimeToAttack)
        {
            if (isFirstShot)
                cameraMovement.resetRotation();
            gunBehavior.Shoot(this);
            currentAmmo--;
            nextTimeToAttack = Time.time + (1f / attackSpeed);
            if (muzzleFlash != null) muzzleFlash.Play();
            ApplyRecoilForce();
        }
    }

    public void Reload()
    {
        Debug.Log("Reloading...");
        isReloading = true;
        transform.DOLocalRotate(new Vector3(45, 0, 0), reloadTime / 2).SetEase(Ease.InOutElastic)
        .OnComplete(() =>
        {
            transform.DOLocalRotate(new Vector3(0, 0, 0), reloadTime / 2).SetEase(Ease.InOutElastic).OnComplete(() =>
            {
                isReloading = false;
                currentAmmo = maxAmmo;
            });
        });
    }

    public void PlayImpactEffect(object sender, ShootEventArgs e)
    {
        if (impactEffect == null) return;
        GameObject impactGO = Instantiate(impactEffect, e.hitPosition, Quaternion.LookRotation(e.hitDirection));
        //Destroy(impactGO, 1f);
    }

    public void ApplyRecoilForce()
    {
        //Weapon recoil
        transform.Rotate(-recoilForce * 10f, 0, 0);
        transform.localPosition -= recoilForce * Vector3.forward;

        //Camera recoil
        cameraMovement.ScreenShake(.1f, .001f);
        if (Mathf.Abs(cameraMovement.getVerticalAngle()) < maxRecoilAngle)
            cameraMovement.RotateBy(new Vector3(-recoilAmount, 0, 0), .5f);
    }

    public void RecoverFromRecoil()
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

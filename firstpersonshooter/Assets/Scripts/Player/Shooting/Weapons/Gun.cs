using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    [Header("Behavior")]
    public ShootBehavior shootBehavior;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;
    [SerializeField] private float reloadTime = 1.5f;
    private bool isReloading;

    [Header("Recoil")]
    [SerializeField] private float recoilForce = 0.1f;
    [SerializeField] private float recoverySpeed = 5;
    [SerializeField] private float maxRecoilAngle = 30;
    [SerializeField] private float recoilAmount = 1;

    [Header("Projectiles")]
    public Transform BulletSpawnPoint;
    public Projectile projectile;
    public float projectileSpeed = 30;

    [Header("VFX")]
    public ParticleSystem muzzleFlash;
    public TrailRenderer bulletTrail;

    protected override void Start()
    {
        base.Start();
        currentAmmo = maxAmmo;
        if(muzzleFlash != null )
            muzzleFlash = Instantiate(muzzleFlash, BulletSpawnPoint.position, Quaternion.identity, transform);
    }

    protected void Update()
    {
        RecoverFromRecoil();
    }

    public override void Attack(bool isFirstShot)
    {
        if (Time.time < nextTimeToAttack || isReloading) return;
        if (isFirstShot) cameraMovement.resetRotation();

        shootBehavior.Shoot(this);
        nextTimeToAttack = Time.time + (1f / attackSpeed);
        if (muzzleFlash != null) muzzleFlash.Play();
        ApplyRecoilForce();
        if (--currentAmmo <= 0) Reload();
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == maxAmmo) return;
        Debug.Log("Reloading...");
        isReloading = true;
        transform.DOLocalRotate(new Vector3(45, 0, 0), reloadTime / 2).SetEase(Ease.InOutElastic)
        .OnComplete(() =>
        {
            transform.DOLocalRotate(new Vector3(0, 0, 0), reloadTime / 2).SetEase(Ease.InOutElastic).OnComplete(() =>
            {
                isReloading = false;
                currentAmmo = maxAmmo;
                cameraMovement.resetRotation();
            });
        });
    }


    protected void ApplyRecoilForce()
    {
        //Weapon recoil
        transform.Rotate(-recoilForce * 10f, 0, 0);
        transform.localPosition -= recoilForce * Vector3.forward;

        //Camera recoil
        cameraMovement.ScreenShake(.1f, .001f);
        if (Mathf.Abs(cameraMovement.getVerticalAngle()) < maxRecoilAngle)
            cameraMovement.RotateBy(new Vector3(-recoilAmount, 0, 0), .1f);
    }

    protected void RecoverFromRecoil()
    {
        //Weapon recoil
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * recoverySpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * recoverySpeed);

        //Camera recoil
        if (!Input.GetMouseButton(0) || isReloading)
        {
            //cameraMovement.StopRotating();
            cameraMovement.transform.localRotation = Quaternion.Slerp(cameraMovement.transform.localRotation, cameraMovement.lastSavedRotation, Time.deltaTime * recoverySpeed);
        }
    }

}

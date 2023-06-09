using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    public event AttackEventHandler OnHit;
    [SerializeField] protected CameraMovement cameraMovement;

    [Header("Attributes")]
    public bool isAutomatic;
    public float attackSpeed;
    public float damage;

    [Header("VFX")]
    public GameObject impactEffect;
    public Texture Symbol;
    [HideInInspector] public float nextTimeToAttack = 0f;

    protected virtual void Start()
    {
        cameraMovement = GetComponentInParent<CameraMovement>();
        OnHit += DealDamage;
        OnHit += PlayImpactEffect;
    }

    public void RaiseOnHitEvent(object sender, AttackEventArgs e)
    {
        OnHit?.Invoke(sender, e);
    }

    /// <summary>
    /// Schieﬂt (beachtet jedoch Feuerrate)
    /// </summary>
    /// <param name="isStart">auf true setzen, falls das der erste Schuss ist (bei automatik)</param>
    public abstract void Attack(bool isStart);

    protected virtual void DealDamage(object sender, AttackEventArgs e)
    {
        Enemy enemy;
        if(e.EnemyHit(out enemy))
            enemy.takeDamage(damage);
    }

    protected void PlayImpactEffect(object sender, AttackEventArgs e)
    {
        if (impactEffect == null) return;
        GameObject impactGO = Instantiate(impactEffect, e.hitPosition, Quaternion.LookRotation(e.hitDirection));
        //Destroy(impactGO, 1f);
    }
}

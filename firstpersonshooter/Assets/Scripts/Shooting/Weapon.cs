using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Weapon : MonoBehaviour
{
    protected event AttackEventHandler OnHit;
    [SerializeField] protected CameraMovement cameraMovement;

    [Header("Attributes")]
    public bool isAutomatic;
    public float attackSpeed;
    public float damage;

    [Header("Effects")]
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected RawImage hitmarker;

    [HideInInspector] public float nextTimeToAttack = 0f;
    protected void Reset()
    {
        cameraMovement = GetComponentInParent<CameraMovement>();
    }

    protected virtual void Start()
    {
        OnHit += DealDamage;
        OnHit += PlayImpactEffect;
        OnHit += showHitmarker;
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
        Enemy enemy = e.hitObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.takeDamage(damage);
        }
    }

    protected void PlayImpactEffect(object sender, AttackEventArgs e)
    {
        if (impactEffect == null) return;
        GameObject impactGO = Instantiate(impactEffect, e.hitPosition, Quaternion.LookRotation(e.hitDirection));
        //Destroy(impactGO, 1f);
    }

    protected void showHitmarker(object sender, AttackEventArgs e)
    {
        if (hitmarker == null) return;
        if(!hitmarker.gameObject.activeSelf)
            StartCoroutine(showGameObject(hitmarker.gameObject, .1f));
    }
    private IEnumerator showGameObject(GameObject obj, float duration)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
    }


}

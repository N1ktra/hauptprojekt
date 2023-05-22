using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public CameraMovement cameraMovement;

    [Header("Attributes")]
    public float attackSpeed;
    public float damage;
    public bool isAutomatic;
    [HideInInspector] public float nextTimeToAttack = 0f;

    public void Reset()
    {
        cameraMovement = GetComponentInParent<CameraMovement>();
    }

    /// <summary>
    /// Schieﬂt (beachtet jedoch Feuerrate)
    /// </summary>
    /// <param name="isFirstShot">auf true setzen, falls das der erste Schuss ist (bei automatik)</param>
    public abstract void Attack(bool isFirstShot);

    public virtual void DealDamage(GameObject obj)
    {
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.takeDamage(damage);
        }
    }
}

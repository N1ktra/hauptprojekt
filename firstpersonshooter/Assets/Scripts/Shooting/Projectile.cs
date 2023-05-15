using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public event Action<ShootEventArgs> OnCollision;
    public Weapon weapon;

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(new ShootEventArgs(collision.gameObject, collision.contacts[0].point, collision.contacts[0].normal));
        //Deal Damage
        //...
        Destroy(gameObject);
        //Oder Explodieren oder so...

    }
}

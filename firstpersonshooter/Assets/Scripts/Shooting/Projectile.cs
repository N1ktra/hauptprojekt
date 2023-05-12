using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Weapon weapon;

    private void OnCollisionEnter(Collision collision)
    {
        //Deal Damage
        //...
        Destroy(gameObject);
        //Oder Explodieren oder so...
    }
}

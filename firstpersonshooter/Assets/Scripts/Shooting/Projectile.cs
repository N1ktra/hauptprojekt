using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Projectile : MonoBehaviour
{
    public event Action<ShootEventArgs> OnCollision;

    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(new ShootEventArgs(collision.gameObject, collision.GetContact(0).point, collision.GetContact(0).normal));
        //...
        Destroy(gameObject);
        //Oder Explodieren oder so...

    }

    /// <summary>
    /// Instanziiert ein neues Projektil
    /// </summary>
    /// <param name="pos">Position des Projektils</param>
    /// <param name="rot">Rotation des Projektils</param>
    /// <returns></returns>
    public GameObject Instantiate(Vector3 pos, Quaternion rot)
    {
        GameObject obj = Instantiate(gameObject, pos, rot);
        return obj;
    }
}

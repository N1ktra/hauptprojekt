using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public event AttackEventHandler OnCollision;

    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision?.Invoke(this, new AttackEventArgs(collision.gameObject, collision.GetContact(0).point, collision.GetContact(0).normal));
        //...
        Destroy(gameObject);
        //Oder Explodieren oder so...

    }

    private Vector3 lastPos = Vector3.zero;
    private void Update()
    {
        if (lastPos == Vector3.zero)
        {
            lastPos = transform.position;
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, lastPos - transform.position), out hit, Vector3.Distance(transform.position, lastPos)))
        {
            OnCollision?.Invoke(this, new AttackEventArgs(hit.transform.gameObject, hit.point, hit.normal));
            Destroy(gameObject);
        }
        lastPos = transform.position;
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

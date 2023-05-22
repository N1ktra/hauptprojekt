using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileBehavior : MonoBehaviour, IGunBehavior
{
    private GameObject cam;
    private void Start()
    {
        cam = Camera.main.gameObject;
    }

    public void Shoot(Gun gun)
    {
        GameObject projectile = gun.projectile.Instantiate(gun.BulletSpawnPoint.position, Quaternion.identity);
        Rigidbody rb_projectile = projectile.GetComponent<Rigidbody>();
        Vector3 direction = cam.transform.forward;
        //Check if it should hit an object -> if so, shoot bullet in correct direction
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            direction = hit.point - gun.BulletSpawnPoint.position;
        }
        rb_projectile.velocity = direction.normalized * gun.projectileSpeed;
        projectile.GetComponent<Projectile>().OnCollision += gun.RaiseOnHitEvent;
    }
}

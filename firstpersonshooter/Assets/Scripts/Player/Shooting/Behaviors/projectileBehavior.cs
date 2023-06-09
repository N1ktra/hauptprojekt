using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gun Behaviors/Projectile")]
public class projectileBehavior : ShootBehavior
{
    public override void Shoot(Gun gun)
    {
        if(gun.projectile == null)
        {
            Debug.LogWarning("Please attach a projectile to the following gun: " + gun.name);
            return;
        }
        Camera cam = Camera.main;
        GameObject projectile = gun.projectile.Instantiate(gun.BulletSpawnPoint.position, Quaternion.identity);
        Rigidbody rb_projectile = projectile.GetComponent<Rigidbody>();
        Vector3 direction = gun.transform.forward;
        //Check if it should hit an object -> if so, shoot bullet in correct direction
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            direction = hit.point - gun.BulletSpawnPoint.position;
        }
        projectile.transform.rotation = Quaternion.LookRotation(direction);
        rb_projectile.velocity = direction.normalized * gun.projectileSpeed;
        projectile.GetComponent<Projectile>().OnCollision += gun.RaiseOnHitEvent;
    }
}

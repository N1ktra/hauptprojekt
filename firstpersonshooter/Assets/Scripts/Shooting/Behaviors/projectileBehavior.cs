using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileBehavior : MonoBehaviour, IWeaponBehavior
{
    public event IWeaponBehavior.ShootEvent OnHit;

    private GameObject cam;
    private void Start()
    {
        cam = Camera.main.gameObject;
    }

    public void Shoot(Weapon weapon)
    {
        GameObject projectile = weapon.projectile.Instantiate(weapon.muzzleFlash.transform.position, Quaternion.identity);
        Rigidbody rb_projectile = projectile.GetComponent<Rigidbody>();
        Vector3 direction = cam.transform.forward;
        //Check if it should hit an object -> if so, shoot bullet in correct direction
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            direction = hit.point - weapon.muzzleFlash.transform.position;
        }
        rb_projectile.velocity = direction.normalized * weapon.projectileSpeed;
        projectile.GetComponent<Projectile>().OnCollision += (ShootEventArgs e) => { OnHit?.Invoke(this, e); };
    }
}
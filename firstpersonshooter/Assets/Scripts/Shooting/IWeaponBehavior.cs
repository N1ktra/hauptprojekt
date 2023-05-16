using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEventArgs
{
    public GameObject hitObject;
    public Vector3 hitPosition;
    public Vector3 hitDirection;

    public ShootEventArgs(GameObject hitObject, Vector3 hitPosition, Vector3 hitDirection)
    {
        this.hitObject = hitObject;
        this.hitPosition = hitPosition;
        this.hitDirection = hitDirection;
    }
}
public interface IWeaponBehavior
{
    public delegate void ShootEvent(object sender, ShootEventArgs e);
    /// <summary>
    /// Has to be raised when the weapon (or Projectile) hits something (like an enemy)
    /// </summary>
    public event ShootEvent OnHit;

    /// <summary>
    /// Führt einen Schuss aus
    /// </summary>
    /// <param name="weapon">Die Waffe, mit der der Schuss gemacht werden soll</param>
    public void Shoot(Weapon weapon);
}


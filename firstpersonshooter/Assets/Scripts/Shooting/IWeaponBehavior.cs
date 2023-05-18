using System;
using UnityEngine;

public interface IWeaponBehavior
{
    /// <summary>
    /// Has to be raised when the weapon (or Projectile) hits something (like an enemy)
    /// </summary>
    public event ShootEventHandler OnHit;

    /// <summary>
    /// Führt einen Schuss aus
    /// </summary>
    /// <param name="weapon">Die Waffe, mit der der Schuss gemacht werden soll</param>
    public void Shoot(Weapon weapon);
}


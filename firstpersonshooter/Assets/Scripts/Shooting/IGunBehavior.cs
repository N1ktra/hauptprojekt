using System;
using UnityEngine;

public interface IGunBehavior
{
    /// <summary>
    /// Has to be raised when the weapon (or Projectile) hits something (like an enemy)
    /// </summary>
    public event ShootEventHandler OnHit;

    /// <summary>
    /// Führt einen Schuss aus
    /// </summary>
    /// <param name="gun">Die Waffe, mit der der Schuss gemacht werden soll</param>
    public void Shoot(Gun gun);
}


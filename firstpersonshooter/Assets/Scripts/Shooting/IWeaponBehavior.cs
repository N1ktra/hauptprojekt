using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponBehavior
{
    /// <summary>
    /// Führt einen Schuss aus
    /// </summary>
    /// <param name="weapon">Die dazugehörige Waffe</param>
    public void Shoot(Weapon weapon);
}

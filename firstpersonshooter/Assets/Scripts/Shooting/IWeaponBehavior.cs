using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponBehavior
{
    /// <summary>
    /// F�hrt einen Schuss aus
    /// </summary>
    /// <param name="weapon">Die dazugeh�rige Waffe</param>
    public void Shoot(Weapon weapon);
}

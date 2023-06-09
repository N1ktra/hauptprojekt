using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : Drop
{
    public Weapon weapon;

    public override void collectDrop(GameObject player)
    {
        Debug.Log("player collected drop");
        WeaponHandler weaponHandler = player.GetComponent<WeaponHandler>();
        Weapon equippedWeapon;
        if(weaponHandler.WeaponIsAlreadyEquipped(weapon, out equippedWeapon))
        {
            if (weapon is Gun && equippedWeapon is Gun)
            {
                (equippedWeapon as Gun).maxAmmo += (weapon as Gun).maxAmmo;
                (equippedWeapon as Gun).currentAmmo = (equippedWeapon as Gun).magSize;
            }
        }
        else
        {
            weaponHandler.addWeapon(weapon);
        }
    }
}

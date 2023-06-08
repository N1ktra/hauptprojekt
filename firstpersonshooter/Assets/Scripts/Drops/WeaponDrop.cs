using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDrop : Drop
{
    public Weapon weapon;

    public override void collectDrop(GameObject player)
    {
        Debug.Log("player collected drop");
        player.GetComponent<WeaponHandler>().addWeapon(weapon);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : Drop
{
    public int amount;

    public override void collectDrop(GameObject player)
    {
        player.GetComponent<PlayerStats>().addHealth(amount);
    }
}

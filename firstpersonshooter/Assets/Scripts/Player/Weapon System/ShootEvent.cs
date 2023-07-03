using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEventArgs : EventArgs
{
    public GameObject hitObject;
    public Vector3 hitPosition;
    public Vector3 hitDirection;

    public AttackEventArgs(GameObject hitObject, Vector3 hitPosition, Vector3 hitDirection)
    {
        this.hitObject = hitObject;
        this.hitPosition = hitPosition;
        this.hitDirection = hitDirection;
    }

    public bool EnemyHit(out Enemy enemy)
    {
        Enemy _enemy = hitObject.GetComponent<Enemy>();
        enemy = _enemy;
        return enemy != null;
    }
}
public delegate void AttackEventHandler(object sender, AttackEventArgs e);

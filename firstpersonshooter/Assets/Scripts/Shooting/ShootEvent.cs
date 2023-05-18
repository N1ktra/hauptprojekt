using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEventArgs : EventArgs
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
public delegate void ShootEventHandler(object sender, ShootEventArgs e);

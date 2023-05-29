using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShootBehavior : ScriptableObject, IShootBehavior
{
    public abstract void Shoot(Gun gun);
}

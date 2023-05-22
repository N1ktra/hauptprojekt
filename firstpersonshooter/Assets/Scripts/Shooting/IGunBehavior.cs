using System;
using UnityEngine;

public interface IGunBehavior
{
    /// <summary>
    /// Führt einen Schuss aus
    /// </summary>
    /// <param name="gun">Die Waffe, mit der der Schuss gemacht werden soll</param>
    public void Shoot(Gun gun);
}


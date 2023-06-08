using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Drop : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            collectDrop(other.gameObject);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// is called when the player collects the drop
    /// </summary>
    /// <param name="player"></param>
    public abstract void collectDrop(GameObject player);
}

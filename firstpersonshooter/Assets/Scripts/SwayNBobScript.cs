using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayNBobScript : MonoBehaviour
{
    public float drag = 2.5f;
    public float smooth = 5;

    private Quaternion localRotation;

    // Start is called before the first frame update
    void Start()
    {
        localRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Sway();
    }

    void Sway()
    {
        float mx = -(Input.GetAxis("Mouse X")) * drag;
        float my = (Input.GetAxis("Mouse Y")) * drag;

        //weapon default rotation transform has to be (0, 0, 0)
        Quaternion newRotation = Quaternion.Euler(localRotation.x + my, localRotation.y + mx, localRotation.z + mx);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, (Time.deltaTime * smooth));
    }

}
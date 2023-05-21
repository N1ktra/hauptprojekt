using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponMovement : MonoBehaviour
{
    [Header("Sway")]
    public float drag = 2.5f;
    public float smooth = 5;

    private Quaternion localRotation;
    private Vector3 localPosition;

    // Start is called before the first frame update
    void Start()
    {
        localRotation = transform.localRotation;
        localPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Sway();
        Bob();
    }

    private void Sway()
    {
        float mx = -(Input.GetAxis("Mouse X")) * drag;
        float my = (Input.GetAxis("Mouse Y")) * drag;

        //weapon default rotation transform has to be (0, 0, 0)
        Quaternion newRotation = Quaternion.Euler(localRotation.x + my, localRotation.y + mx, localRotation.z + mx);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, (Time.deltaTime * smooth));
    }

    [Header("Bob")]
    // Variables for weapon bobbing
    public float bobbingSpeed = 0.05f;  // Speed of the bobbing motion
    public float bobbingAmount = 0.01f; // Amount of bobbing motion

    // Variables for timing and position calculation
    private float timer = 0.0f;
    private void Bob()
    {
        // Calculate the vertical position offset based on time
        float waveslice = 0.0f;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveslice = Mathf.Sin(timer);
            timer += bobbingSpeed;
            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        waveslice = waveslice * bobbingAmount;

        // Apply the calculated offset to the weapon's position
        transform.localPosition = new Vector3(localPosition.x, localPosition.y + waveslice, localPosition.z);
    }

}
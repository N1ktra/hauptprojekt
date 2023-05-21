using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class WeaponMovement : MonoBehaviour
{
    Vector2 mouseInput;
    Vector2 keyboardInput;
    private Quaternion localRotation;
    private Vector3 localPosition;

    [Header("Sway")]
    public float drag = 2.5f;
    public float smooth = 5;

    [Header("Bob")]
    public float bobbingSpeed = 0.05f;  // Speed of the bobbing motion
    public float bobbingAmount = 0.01f; // Amount of bobbing motion
    private Sequence bobbing;


    void Start()
    {
        localRotation = transform.localRotation;
        localPosition = transform.localPosition;
    }

    void Update()
    {
        keyboardInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        MouseSway();
        KeyboardSway();
        Bob();
        recover();
    }

    private void MouseSway()
    {
        float mx = -mouseInput.x * drag;
        float my = mouseInput.y * drag;

        //weapon default rotation transform has to be (0, 0, 0)
        Quaternion newRotation = Quaternion.Euler(localRotation.x + my, localRotation.y + mx, localRotation.z + mx);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, (Time.deltaTime * smooth));
    }

    private void KeyboardSway()
    {
        float mx = -keyboardInput.x * drag;
        float my = keyboardInput.y * drag;
        Quaternion newRotation = Quaternion.Euler(localRotation.x + my, localRotation.y + mx, localRotation.z + mx);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, (Time.deltaTime * smooth));
    }

    private void Bob()
    {
        if (keyboardInput == Vector2.zero)
        {
            bobbing.Pause();
        }

        if(bobbing == null || !bobbing.IsActive())
        {
            bobbing = DOTween.Sequence();
            bobbing.Append(transform.DOLocalMove(transform.localPosition - transform.up * bobbingAmount, bobbingSpeed));
            bobbing.Append(transform.DOLocalMove(transform.localPosition + transform.up * bobbingAmount, bobbingSpeed * 3));
            bobbing.SetAutoKill(false);
            bobbing.Pause();
        }

        if (keyboardInput != Vector2.zero && !bobbing.IsPlaying())
        {
            bobbing.Restart();
        }

    }

    private void recover()
    {
        if (keyboardInput == Vector2.zero)
            transform.localPosition = Vector3.Lerp(transform.localPosition, localPosition, Time.deltaTime * 10f);
    }

}
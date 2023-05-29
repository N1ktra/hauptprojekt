using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Camera fpsCamera;
    public Quaternion lastSavedRotation = Quaternion.identity;

    private void Reset()
    {
        fpsCamera = Camera.main;
    }

    public float getVerticalAngle()
    {
        var vecA = transform.forward;
        var vecB = transform.parent.forward;

        float angle = -Vector3.SignedAngle(vecA, vecB, transform.right);
        return angle;
    }
    public void resetRotation()
    {
        float angle = getVerticalAngle();
        Quaternion fpsRot = fpsCamera.transform.rotation;
        transform.localRotation = Quaternion.identity;
        lastSavedRotation = Quaternion.identity;
        fpsCamera.transform.rotation = fpsRot;
        //adjust the Camera that it also looks at the correct new position
        fpsCamera.GetComponent<FirstPersonLook>().SetVerticalOrientation(angle);
    }

    private Coroutine runningRotationCoroutine;
    public void RotateBy(Vector3 rotation, float duration)
    {
        StopRotating();
        runningRotationCoroutine = StartCoroutine(rotateSmooth(Quaternion.Euler(rotation) * transform.localRotation, duration));
    }
    public void RotateTo(Quaternion rotation, float duration, Action callback)
    {
        StopRotating();
        runningRotationCoroutine = StartCoroutine(rotateSmooth(rotation, duration));
    }
    private IEnumerator rotateSmooth(Quaternion targetRotation, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void StopRotating()
    {
        if (runningRotationCoroutine != null)
            StopCoroutine(runningRotationCoroutine);
    }

    public void ScreenShake(float duration, float magnitude)
    {
        StartCoroutine(performShake(duration, magnitude));
    }

    private IEnumerator performShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            transform.localPosition += new Vector3 (x, y,0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
    
}

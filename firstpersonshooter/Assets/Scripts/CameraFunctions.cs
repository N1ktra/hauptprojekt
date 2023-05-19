using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFunctions : MonoBehaviour
{
    [SerializeField] private Vector3 startRotation;
    [SerializeField] private Vector3 rotationApplied = Vector3.zero;

    private void Start()
    {
        startRotation = transform.rotation.eulerAngles;
    }

    public Vector3 getRotation()
    {
        return startRotation + rotationApplied;
    }

    public void Rotate(Vector3 rotation)
    {
        rotationApplied += rotation;
        transform.Rotate(rotation);
    }

    public void RotateTo(Quaternion rotation)
    {
        rotationApplied += (Quaternion.Inverse(transform.localRotation) * rotation).eulerAngles;
        transform.localRotation = rotation;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(performShake(duration, magnitude));
    }

    private IEnumerator performShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition += new Vector3 (x, y,0);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
    
}

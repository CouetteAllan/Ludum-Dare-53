using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float duration = 1.0f;

    public void ShakeCamera(float shakeMultiplier)
    {
        StartCoroutine(Shaking(shakeMultiplier));
    }

    private IEnumerator Shaking(float shakeMultiplier)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strenght = curve.Evaluate(elapsedTime/duration);
            transform.position = startPosition + Random.insideUnitSphere * strenght * shakeMultiplier;
            yield return null;
        }

        transform.position = startPosition;
    }
}

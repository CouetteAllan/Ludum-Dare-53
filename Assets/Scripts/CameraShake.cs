using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float duration = 1.0f;
    public bool IsShaking { get; private set; }

    public void ShakeCamera(float shakeMultiplier, float duration)
    {
        IsShaking = true;
        StartCoroutine(Shaking(shakeMultiplier, duration));
        
    }

    public void ShakeCamera(float shakeMultiplier) 
    {
        IsShaking = true;
        StartCoroutine(Shaking(shakeMultiplier, this.duration));

    }

    private IEnumerator Shaking(float shakeMultiplier, float duration)
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

        IsShaking = false;
        transform.position = startPosition;
    }
}

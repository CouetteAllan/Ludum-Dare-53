using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCamera : MonoBehaviour
{
    public List<Transform> targets;

    public Vector3 offset;

    public float smoothTime = 0.5f;

    public float minZoom = 40f;
    public float maxZoom = 10f;
    public float zoomlimiter = 50f;


    public Vector2 min, max;

    private Vector3 velocity;
    private Camera cam;

    private void Start()
    {
        cam = this.GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if(targets.Count == 0)
            return;

        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomlimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);


        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y), transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    private float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for(int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.size.x;
    }

    private Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
            return targets[0].position;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for(int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}

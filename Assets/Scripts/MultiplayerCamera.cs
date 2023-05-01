using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerCamera : MonoBehaviour
{
    private List<Transform> targets = new List<Transform>();

    [SerializeField] private Vector3 offset;
    
    [SerializeField] private float smoothTime = 0.5f;
     
    [SerializeField] private float minZoom = 40f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float zoomlimiter = 50f;
    
    
    //[SerializeField] private Vector2 min, max;

    private Vector3 velocity;
    private Camera cam;
    private CameraShake camShake;   

    private void Start()
    {
        cam = Helpers.Camera;
        camShake = this.GetComponent<CameraShake>();
        GameManager.OnStateChanged += GameManager_OnStateChanged;
        
    }

    private void GameManager_OnStateChanged(GameState state)
    {
        if(state == GameState.DebutGame)
        {
            foreach (var p in MultiPlayerManager.Instance.Players)
            {
                targets.Add(p.transform);
            }
        }
    }

    private void LateUpdate()
    {
        if(targets.Count == 0 || camShake.IsShaking)
            return;

        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomlimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime);


        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        //transform.position = new Vector3(Mathf.Clamp(transform.position.x, min.x, max.x), Mathf.Clamp(transform.position.y, min.y, max.y), transform.position.z);
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

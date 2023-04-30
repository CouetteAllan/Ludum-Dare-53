using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleHandle : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private FXStruct[] SFX;
    
    public void PlayEffect(string fxName, bool flip = false)
    {
        FXStruct particle;
        try
        {
            particle = SFX.First(b => b.name == fxName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Particle Effect name not found in array" + e.Message);
            throw;
        }
        
        var go = Instantiate(particle.particlePrefab,particle.playPosition.position,Quaternion.identity, particle.attachToParent? particle.playPosition : null);
        if (flip)
        {
            Vector3 scale = go.transform.localScale;
            scale.x *= -1;
            go.transform.localScale = scale;
        }
        go.GetComponent<Animator>().SetTrigger("Play");
        
    }
}

[Serializable]
public struct FXStruct
{
    public string name;
    public GameObject particlePrefab;
    public Transform playPosition;
    public bool attachToParent;
}

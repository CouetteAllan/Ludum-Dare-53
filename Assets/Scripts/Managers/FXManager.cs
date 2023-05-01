using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FXManager : Singleton<FXManager>
{
    [SerializeField] private List<VFXdatas> vfxList = new List<VFXdatas>();

    private List<GameObject> instantiatedVFXList = new List<GameObject>();


    public void PlayEffect(string effectName, Vector3 worldPos)
    {
        var vfx = Instantiate(vfxList.First(b => effectName == b.FXName).FXprefab, worldPos, Quaternion.identity);
        instantiatedVFXList.Add(vfx);
    }

    public void DestroyEffect()
    {
        foreach (var item in instantiatedVFXList)
        {
            Destroy(item.gameObject);
        }
    }
}

[Serializable]
public struct VFXdatas
{
    public string FXName;
    public GameObject FXprefab;
}
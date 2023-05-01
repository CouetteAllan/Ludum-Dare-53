using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXAutoDestroy : MonoBehaviour
{
    public void DestroyFX()
    {
        Destroy(this.gameObject);
    }

    public void PlaySoundFX(string soundName)
    {
        SoundManager.Instance.Play(soundName);
    }
}

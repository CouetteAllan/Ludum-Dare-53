using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPassThrough : MonoBehaviour
{
    private bool canPass = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.OnDropDown += Player_OnDropDown;
        }
    }

    private void Player_OnDropDown()
    {
        throw new System.NotImplementedException();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {

        }
    }
}

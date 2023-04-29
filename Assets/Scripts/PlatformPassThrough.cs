using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPassThrough : MonoBehaviour
{
    private bool canPass = false;
    private Collider2D playerCollider;
    private Collider2D platformCollider;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
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
            playerCollider = collision.collider;
        }
    }

    private void Player_OnDropDown()
    {
        StartCoroutine(CollisionCoroutine());
    }

    private IEnumerator CollisionCoroutine()
    {
        float timerNoCollision = 0.5f;
        Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
        yield return new WaitForSeconds(timerNoCollision);
        Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            player.OnDropDown -= Player_OnDropDown;
        }
    }
}

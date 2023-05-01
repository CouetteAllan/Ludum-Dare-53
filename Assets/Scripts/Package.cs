using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour,IPickUpItem
{
    private PlayerScript ownedPlayer;
    public PlayerScript CurrentOwnedPlayer { get { return ownedPlayer; } }
    public GameObject triangle;
    public float speed = 0.05f, timer = 1f;
    private float privTimer;

    [SerializeField] public Rigidbody2D Rigidbody2D { get; private set; }
    public void PickUp(PlayerScript player)
    {
        this.Rigidbody2D = GetComponent<Rigidbody2D>();
        ownedPlayer = player;
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        player.AttachPackage(this);
        player.OnPackageDrop += DropPackage;
        //Play sound
        //Play feedback visuels
        //Disable toutes les collisions.
    }

    public void DropPackage(bool gotHit)
    {
        //Réactiver toutes les collisions du package lorsque le joueur lâche le paquet
        this.gameObject.GetComponent<Collider2D>().enabled = true;

        if(gotHit)
        {
            Vector2 dir = new Vector2(Random.Range(-1.0f, 1.0f), 1);
            float pushForce = 40f;
            //ajouter impulsion du paquet random ?
            this.Rigidbody2D.AddForce(dir.normalized * pushForce,ForceMode2D.Impulse);
            //faire en sorte que le paquet n'appartienne plus à personne
            ownedPlayer.OnPackageDrop -= DropPackage;
            ownedPlayer = null;
            return;
        }
        ownedPlayer.OnPackageDrop -= DropPackage;
    }

    private void Start()
    {
        privTimer = timer;
    }

    private void Update()
    {
        privTimer -= Time.deltaTime;

        triangle.transform.position  += Vector3.up * speed * Time.deltaTime;
        if(privTimer <= 0)
        {
            speed *= -1;
            privTimer = timer;
        }
    }

}

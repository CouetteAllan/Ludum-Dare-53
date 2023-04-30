using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour,IPickUpItem
{
    private PlayerScript ownedPlayer;
    public PlayerScript CurrentOwnedPlayer { get { return ownedPlayer; } }

    [SerializeField] public Rigidbody2D Rigidbody2D { get; private set; }
    public void PickUp(PlayerScript player)
    {
        this.Rigidbody2D = GetComponent<Rigidbody2D>();
        ownedPlayer = player;
        player.AttachPackage(this);
        player.OnPackageDrop += DropPackage;
        //Play sound
        //Play feedback visuels
        //Disable toutes les collisions.
        this.gameObject.GetComponent<Collider2D>().enabled = false;
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
            return;
        }
        ownedPlayer.OnPackageDrop -= DropPackage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Spot spot = collision.gameObject.GetComponent<Spot>();
        if(spot != null)
        {
            spot.parentBuilding.ChangeState(State.ColoredP1);
        }
    }
}

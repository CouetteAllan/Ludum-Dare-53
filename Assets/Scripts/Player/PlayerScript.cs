using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Animator animator;
    public Animator Animator { get { return animator; } }

    [SerializeField] private Transform attachPackagePointTransform;
    private bool hasPackage = false;
    private Package ownedPackage = null;
    private float basePackageGravityScale;
    public event Action<bool> OnPackageDrop;
    public event Action OnPackagePickUp;
    public bool HasRecentlyLostPackage { get; set; }

    [Tooltip("Timer pour set up le cooldown avant de reprendre le colis si on l'a perdu récemment")]
    [SerializeField] private float timerBeforePackagePickUp = 0.5f;
    private float nextTimerPackagePickUp = 0.0f;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnHit()
    {
        //Instancier jouer l'FX de hit.
        //Stun

        //Lacher le package si y a package
        if(hasPackage)
        {
            hasPackage = false;
            bool gothit = true;
            OnPackageDrop?.Invoke(gothit);
        }
    }

    public void PackageDropped()
    {
        if (!hasPackage)
            return;

        //Drop le paquet (à ses pieds ?)
        ownedPackage.transform.SetParent(null);
        ownedPackage.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        ownedPackage.Rigidbody2D.gravityScale = basePackageGravityScale;
        HasRecentlyLostPackage = true;
        nextTimerPackagePickUp = Time.time + timerBeforePackagePickUp;
        hasPackage = false;
        var gotHit = false;
        OnPackageDrop?.Invoke(gotHit);
    }

    public void AttachPackage(Package package)
    {
        OnPackagePickUp?.Invoke();
        ownedPackage = package;
        hasPackage = true;

        package.Rigidbody2D.transform.SetParent(attachPackagePointTransform, true);
        package.Rigidbody2D.position = attachPackagePointTransform.position;
        package.Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;

        var packageRB = package.Rigidbody2D;
        basePackageGravityScale = packageRB.gravityScale;
        packageRB.gravityScale = 0.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IPickUpItem>(out IPickUpItem item))
        {
            if((item.GetType() == typeof(Package)) && !CanPickUpPackage())
            {
                return;
            }
            item.PickUp(this);
        }
    }

    private bool CanPickUpPackage()
    {
        if(Time.time >= nextTimerPackagePickUp)
        {
            HasRecentlyLostPackage = false;
            return true;
        }
        return false;
    }
}

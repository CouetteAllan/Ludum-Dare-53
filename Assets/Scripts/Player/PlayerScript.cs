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

    [Tooltip("Timer pour set up le cooldown avant de reprendre le colis si on l'a perdu r�cemment")]
    [SerializeField] private float timerBeforePackagePickUp = 0.5f;
    private float nextTimerPackagePickUp = 0.0f;

    //� mettre dans des param�tres syst�me pour que �a soit modifiable dans les r�gles du jeu
    [SerializeField] private float stunTime = 0.2f;
    private bool isStun = false;

    [Space(3.0f)]
    [SerializeField] private ParticleHandle VFX;

    [Space(3.0f)]
    [SerializeField] private BoxCollider2D hitBox;
    [SerializeField] private BoxCollider2D hurtBox;

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
        VFX.PlayEffect("Hit");
        //Stun
        StartCoroutine(StunState());
        //Lacher le package si y a package
        PackageDropped(gotHit: true);
    }

    IEnumerator StunState()
    {
        isStun = true;
        yield return new WaitForSeconds(stunTime);
        isStun = false;
    }

    public void PackageDropped(bool gotHit)
    {
        if (!hasPackage)
            return;

        //Drop le paquet (� ses pieds ?)
        ownedPackage.transform.SetParent(null);
        ownedPackage.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        ownedPackage.Rigidbody2D.gravityScale = basePackageGravityScale;
        HasRecentlyLostPackage = true;
        nextTimerPackagePickUp = Time.time + timerBeforePackagePickUp;
        hasPackage = false;
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
        
        
        if(collision.TryGetComponent<PlayerScript>(out PlayerScript playerScript) && hitBox.IsTouching(playerScript.hurtBox))
        {
            playerScript.OnHit();
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    /// <summary>
    /// Fire when any player scores a point and take in parameter and int for the player index (0 or 1)
    /// </summary>
    public static event Action<int> OnPlayerScore;

    private Animator animator;
    public Animator Animator { get { return animator; } }

    [SerializeField] private SelectedCharacterData characterData;
    public int PlayerIndex { get; private set; } = -1;
    
    public SelectedCharacterData CharacterData { get { return characterData; } }
    [SerializeField] private Transform attachPackagePointTransform;
    private bool hasPackage = false;
    private Package ownedPackage = null;
    public event Action<bool> OnPackageDrop;
    public event Action OnPackagePickUp;
    public bool HasRecentlyLostPackage { get; set; }

    [Tooltip("Timer pour set up le cooldown avant de reprendre le colis si on l'a perdu récemment")]
    [SerializeField] private float timerBeforePackagePickUp = 0.5f;
    private float nextTimerPackagePickUp = 0.0f;

    //à mettre dans des paramètres système pour que ça soit modifiable dans les règles du jeu
    [SerializeField] private float stunTime = 0.2f;
    private bool isStun = false;
    public bool IsStun { get { return isStun; } } 

    [Space(3.0f)]
    [SerializeField] private ParticleHandle VFX;

    [Space(3.0f)]
    [SerializeField] private BoxCollider2D hitBox;
    [SerializeField] private BoxCollider2D hurtBox;

    private GameObject graphObject;

    private const float BASE_PACKAGE_GRAVITY = 3.0f;

    private PlayerController controller;
    private float baseSpeed;

    private CameraShake camRef;

    private PlatformEffector2D platform;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Init(SelectedCharacterData datas)
    {
        characterData = datas;

        this.animator.runtimeAnimatorController = characterData.animController;
        graphObject = this.transform.GetChild(0).GetChild(0).gameObject;
        graphObject.GetComponent<SpriteRenderer>().color = characterData.spriteColor;
        PlayerIndex = this.gameObject.GetComponent<PlayerInput>().playerIndex;
        controller = GetComponent<PlayerController>();
        controller.OnDropDown += PassThroughPlatform;
    }

    private void Start()
    {
        camRef = Helpers.Camera.GetComponent<CameraShake>();
    }

    private void OnHit()
    {
        //Instancier jouer l'FX de hit.
        animator.SetTrigger("Hit");
        VFX.PlayEffect("Hit");
        //Stun
        StartCoroutine(StunState());
        //Lacher le package si y a package
        PackageDropped(gotHit: true);
        SoundManager.Instance.Play("Hit");
        camRef.ShakeCamera(shakeMultiplier: 0.8f,duration: 0.5f);
    }

    IEnumerator StunState()
    {
        graphObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        isStun = true;
        yield return new WaitForSeconds(stunTime);
        isStun = false;
        graphObject.GetComponent<SpriteRenderer>().color = characterData.spriteColor;
    }

    public void PackageDropped(bool gotHit)
    {
        if (!hasPackage)
            return;

        Debug.Log("DROP IT JSDFL");
        //Drop le paquet (à ses pieds ?)
        ownedPackage.transform.SetParent(null);
        ownedPackage.Rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        ownedPackage.Rigidbody2D.gravityScale = BASE_PACKAGE_GRAVITY;
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

        package.Rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        package.Rigidbody2D.transform.SetParent(attachPackagePointTransform, true);
        package.Rigidbody2D.position = attachPackagePointTransform.position;
        package.gameObject.transform.position = attachPackagePointTransform.position;

        var packageRB = package.Rigidbody2D;
        packageRB.gravityScale = 0.0f;
    }

    public void PlaySound(string soundName)
    {
        SoundManager.Instance.Play(soundName);
    }

    public void PlayParticle(string particleName)
    {
        VFX.PlayEffect(particleName);
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

        if(collision.TryGetComponent<Spot>(out Spot spot))
        {
            if (!hasPackage)
                return;
            PackageDropped(gotHit: false);
            spot.parentBuilding.ScorePlayer(this);
            OnPlayerScore?.Invoke(PlayerIndex);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<PlatformEffector2D>(out PlatformEffector2D effector))
        {
            this.platform = effector;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<PlatformEffector2D>(out PlatformEffector2D effector))
        {
            this.platform = null;
        }
    }

    private void PassThroughPlatform()
    {
        StartCoroutine(DisableCollision());
    }

    private void OnDisable()
    {
        controller.OnDropDown -= PassThroughPlatform;
    }

    private IEnumerator DisableCollision()
    {
        var platformCollider = platform.gameObject.GetComponent<Collider2D>();
        var playerCollider = this.GetComponent<Collider2D>();

        Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
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

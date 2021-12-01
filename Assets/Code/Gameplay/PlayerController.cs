using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour {
    public float movementSpeed;
    [SerializeField] private bool canClimb = false;
    [SerializeField] private bool canClimbDown = false;
    private float currentLadderY = 0;
    public float climbingSpeed;

    [HideInInspector] public int ammoCount;
    [HideInInspector] public string projectileType = "Vine";
    float movementX = 0;
    float movementY = 0;
    bool shieldActive = false;
    string activeAnimation = "Idle";
    bool isShooting = false;
    int maxVines = 1;
    bool stickyVines = false;
    bool knockedFromLadder = false;
    bool canStep = true; //temp
    public int combo = 0;

    public AudioSource audioSrc;
    public AudioClip[] audioClips;
    public AudioClip hitSound;
    public AudioClip reloadSound;

    Rigidbody2D rb;
    BoxCollider2D bc;
    Animator animator;
    SpriteRenderer[] spriteRenderers;
    [SerializeField] private LayerMask layerMask;
    public GameObject grapplePrefab;
    public GameObject stickyVinePrefab;
    RapidFireManager rapidFire;
    Coroutine lastRoutine = null;
    private int health = 3;
    [SerializeField] public bool playerHit {get; set;}
    // prevent gravityScale from turning back too soon:
    [SerializeField] private bool hitOffGroundOffset = false;
    [SerializeField] private float invincibilityDurationSeconds;
    [SerializeField] private float invincibilityDeltaTime = 0.15f;

    private PlayerUI hud;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
        rapidFire = GetComponent<RapidFireManager>();

        hud = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();

        SetActiveAnimation("Idle");
    }

    void Update() {
        hud.SetAmmo(ammoCount);
        hud.SetHealth(health);

        if (!transform.root.Find("UI").Find("UIController").GetComponent<UIController>().paused && health >= 1) {

            if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) && ammoCount > 0 && IsGrounded() && projectileType == "RapidFire" && lastRoutine == null) {
                lastRoutine = StartCoroutine(HoldingAttack());
            } else if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) && ammoCount > 0 && IsGrounded() && lastRoutine == null) {
                Attack();
            } else if ((Input.GetButtonUp("Fire1") || Input.GetButtonUp("Jump") || projectileType != "RapidFire") && lastRoutine != null && animator.GetBool("rapidFiring")) {
                StopCoroutine(lastRoutine);
                lastRoutine = null;
                DisableShooting();
                animator.SetBool("rapidFiring", false);
            }

            if (knockedFromLadder) {
                canClimb = false;
                canClimbDown = false;
                knockedFromLadder = false;
            }
        }
        
    }

    void FixedUpdate() {
        if (!transform.root.Find("UI").Find("UIController").GetComponent<UIController>().paused && health >= 1) {
            // Horizontal movement:
            if (Input.GetAxisRaw("Horizontal") != 0 && !isShooting) {
                Walk();
            } else {
                animator.SetBool("isRunning", false);
                if (!isShooting) {
                     SetActiveAnimation("Idle");
                }
            }
            // if (animator.GetCurrentAnimatorStateInfo(0).IsTag("idle") && !animator.GetBool("rapidFiring")) {
            //      SetActiveAnimation("Idle");
            // }

            // Can player climb and are they trying to climb:
            if (canClimb && Input.GetAxisRaw("Vertical") != 0 && !isShooting) {

                // Can player climb down, is the ladder below them and are they attempting to climb down:
                if (canClimbDown && currentLadderY < bc.bounds.min.y && Input.GetAxisRaw("Vertical") < 0) {
                    // Turn player into a semisolid able to go through platforms:
                    gameObject.layer = LayerMask.NameToLayer("SemisolidPlayer");

                // Is the player on the ground or is the ladder they're climbing above them:
                } else if (IsGrounded() || currentLadderY > transform.localPosition.y) {
                    // Turn player back into a solid object and disable canClimbDown:
                    canClimbDown = false;
                    gameObject.layer = LayerMask.NameToLayer("Player");
                }
                Climb();

            // If the player is unable to climb anymore, turn it's gravityScale back on:
            } else if (canClimb && animator.GetBool("isClimbing") && !IsGrounded()) {
                animator.speed = 0;
            } else if (!canClimb && !playerHit || (IsGrounded() && !hitOffGroundOffset)) {
                rb.gravityScale = 1;
                animator.speed = 1;
                animator.SetBool("isClimbing", false);
            }
        }

        Flip();
    }

    void Walk() {
        if (!playerHit || !hitOffGroundOffset) {
            movementX = Input.GetAxisRaw("Horizontal") * movementSpeed;
            transform.position += new Vector3(movementX, 0, 0) * Time.deltaTime;

            animator.SetBool("isRunning", true);
            SetActiveAnimation("Running");
        }
    }

    void PlayFootstepSound() {
        audioSrc.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSrc.volume = ApplicationSettings.SoundVolume() * 0.2f;
        audioSrc.Play();
    }

    IEnumerator PlayHitSound() {
        audioSrc.clip = hitSound;
        audioSrc.volume = ApplicationSettings.SoundVolume() * 0.175f;
        audioSrc.Play();
        yield return new WaitForSeconds(hitSound.length);;
    }

    IEnumerator PlayReloadSound() {
        audioSrc.clip = reloadSound;
        audioSrc.volume = ApplicationSettings.SoundVolume() * 0.2f;
        audioSrc.Play();
        yield return new WaitForSeconds(reloadSound.length);
    }

    IEnumerator FootstepCooldown() { //temp
        canStep = false;
        yield return new WaitForSeconds(0.45f);
        canStep = true;
    }

    void Climb() {
        if (!playerHit || !hitOffGroundOffset) {
            animator.speed = 1;
            animator.SetBool("isClimbing", true);
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
            movementY = Input.GetAxisRaw("Vertical") * climbingSpeed;
            transform.position += new Vector3(0, movementY, 0) * Time.deltaTime;
            SetActiveAnimation("Climbing");
        }
    }

    void Attack() {
        GameObject grappleObject;
        switch (projectileType) {
            case "Vine":
                if (transform.parent.GetComponent<LevelManager>().CountVines() < maxVines && !isShooting) {
                    isShooting = true;
                    //SetActiveAnimation("Shooting");
                    animator.SetTrigger("shot");
                    ChangeAmmoCount(-1);
                    if (stickyVines) {
                        grappleObject = Instantiate(stickyVinePrefab, new Vector3(transform.position.x, transform.position.y - (grapplePrefab.GetComponent<SpriteRenderer>().size.y/2), 0f), Quaternion.identity) as GameObject;
                    } else {
                        grappleObject = Instantiate(grapplePrefab, new Vector3(transform.position.x, transform.position.y - (grapplePrefab.GetComponent<SpriteRenderer>().size.y/2), 0f), Quaternion.identity) as GameObject;
                    }
                    
                    grappleObject.transform.parent = transform.parent;
                    grappleObject.GetComponent<Grapple>().stickyVines = stickyVines;
                } else if (stickyVines) {
                    transform.parent.GetComponent<LevelManager>().DestroyUnmovingVines();
                }
                break;

            case "RapidFire":
                isShooting = true;
                //SetActiveAnimation("Shooting");
                animator.SetTrigger("shot fast");
                animator.SetBool("rapidFiring", true);
                rapidFire.Fire();
                break;

            default:
                Debug.Log("Invalid projectile type");
                break;
        }
    }

    IEnumerator HoldingAttack() {
        while(true) {
            if (IsGrounded()) {
                Attack();
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    void ChangeAmmoCount(int amount) {
        ammoCount += amount;
        hud.SetAmmo(ammoCount);
    }

    void ChangeHealth(int amount) {
        health += amount;
        hud.SetHealth(health);
    }

    void OnTriggerEnter2D(Collider2D col) {

        if (col.gameObject.tag == "Ladder") {
            canClimb = true;
            canClimbDown = col.gameObject.transform.localPosition.y < transform.localPosition.y;
            currentLadderY = col.bounds.center.y - col.bounds.extents.y;
        }

        if (col.gameObject.tag == "Ball" && !playerHit) {
            HitPlayer(col.gameObject.transform.localPosition.x);
            combo = 0;
        }

    }

    // Debug for ladders
    // void OnDrawGizmos() {
    //     Gizmos.DrawLine(new Vector2(0, currentLadderY), new Vector2(10, currentLadderY));
    // }

    public void HandleDrops(GameObject gameObject) {
        switch (gameObject.tag) {
            case "AmmoDrop":    StartCoroutine(PlayReloadSound());
                                if (ammoCount > 5) {
                                    ChangeAmmoCount(Random.Range(1, 4));
                                    break;
                                } else if (ammoCount > 0) {
                                    ChangeAmmoCount(Random.Range(2, 5));
                                    break;
                                } else {
                                    ChangeAmmoCount(Random.Range(3, 6));
                                    break;
                                }
                                
            case "TimeFreeze":  StartCoroutine(transform.parent.GetComponent<LevelManager>().FreezeBubbles());
                                break;
            case "DamageAll":   transform.parent.GetComponent<LevelManager>().DamageAllBubbles();
                                break;
            case "Shield":      ActiveShield();
                                break;
            case "DoubleVines": maxVines = 2;
                                stickyVines = false;
                                projectileType = "Vine";
                                break;
            case "StickyVines": stickyVines = true;
                                maxVines = 1;
                                projectileType = "Vine";
                                break;
            case "RapidFire":   StartCoroutine(rapidFire.Activate());
                                stickyVines = false;
                                maxVines = 1;
                                break;
            case "TimerBoost":  int randTime = (int)Mathf.Round(Random.Range(10, 21) / 5f) * 5;
                                transform.root.Find("UI/Canvas/PlayerUI/Timer/Timertext").GetComponent<TimerController>().AddToTimer(randTime);
                                transform.root.Find("UI/Canvas/PopupTextManager").GetComponent<PopupTextManager>().NewPopupText("+" + (randTime).ToString() + "s", transform.position);
                                break;
        }
    }

    void ActiveShield() {
        // turn blue here or something
        shieldActive = true;
    }
    
    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Ladder") {
            canClimb = false;
            canClimbDown = false;
            SetActiveAnimation("Idle");
            animator.SetBool("isClimbing", false);
        }
    }

    public void HitPlayer(float enemyX) {
        if (playerHit) {
            return;
        }

        if (shieldActive) {
            shieldActive = false;
            StartCoroutine(CreateIFrames());
            return;
        }

        ChangeHealth(-1);
        StartCoroutine(PlayHitSound());
        knockedFromLadder = true;
        int dir = enemyX < transform.localPosition.x ? 1 : -1;
        rb.gravityScale = 0.5f;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(2.5f * dir, 3.25f), ForceMode2D.Impulse);
        SetActiveAnimation("Hit");


        // Player dead:
        if (health <= 0) {
            Debug.Log("Player dead");
            GameObject.Find("LevelManager").GetComponent<LevelManager>().LevelLose();
            //GetComponentInChildren<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        StartCoroutine(CreateIFrames());
    }

    bool IsGrounded() {
        Vector2 rayOrigin = bc.bounds.center - bc.bounds.extents + new Vector3(bc.bounds.extents.x, 0);
        Vector2 raycastSize = new Vector2(bc.bounds.size.x, bc.bounds.size.y * 0.05f);
        RaycastHit2D raycastHit = Physics2D.BoxCast(rayOrigin, raycastSize, 0f, Vector2.zero, 0, layerMask);

        //DEBUG:
        Color rayColor;
        if (raycastHit.collider != null) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        
        Debug.DrawRay(new Vector2(rayOrigin.x - raycastSize.x/2, rayOrigin.y + raycastSize.y/2), new Vector3(raycastSize.x, 0), rayColor);  // top
        Debug.DrawRay(new Vector2(rayOrigin.x - raycastSize.x/2, rayOrigin.y + raycastSize.y/2), new Vector3(0, -raycastSize.y), rayColor); // left
        Debug.DrawRay(new Vector2(rayOrigin.x + raycastSize.x/2, rayOrigin.y + raycastSize.y/2), new Vector3(0, -raycastSize.y), rayColor); // right
        Debug.DrawRay(new Vector2(rayOrigin.x - raycastSize.x/2, rayOrigin.y - raycastSize.y/2), new Vector3(raycastSize.x, 0), rayColor);  // bottom
        
        return raycastHit.collider != null;
    }

    private IEnumerator CreateIFrames() {
        playerHit = true;
        bool flash = false;
        hitOffGroundOffset = true;

        for (float i = 0; i < invincibilityDurationSeconds; i += invincibilityDeltaTime) {
            TurnInvisible(flash);
            flash = !flash;
            yield return new WaitForSeconds(invincibilityDeltaTime);
            if (IsGrounded()) {
                hitOffGroundOffset = false;
                SetActiveAnimation("Idle");
            }
        }

        TurnInvisible(true);
        playerHit = false;
    }

    private void TurnInvisible(bool boolean) {
        transform.Find(activeAnimation).gameObject.SetActive(boolean);
    }

    void Flip () {
        float oldX = Mathf.Abs(transform.localScale.x);
        float oldY = Mathf.Abs(transform.localScale.y);
        if (movementX < 0 || rb.velocity.x < 0) {
            transform.localScale = new Vector2(oldX, oldY);
        } else if (movementX > 0 || rb.velocity.x > 0) {
            transform.localScale = new Vector2(-oldX, oldY);
        }
    }

    public void SetActiveAnimation(string newAnimation = "Idle") {
        if (hitOffGroundOffset) { // return if being hit
            return;
        }
        transform.Find("Idle").gameObject.SetActive(false);
        transform.Find("Hit").gameObject.SetActive(false);
        transform.Find("Shooting").gameObject.SetActive(false);
        transform.Find("Climbing").gameObject.SetActive(false);
        transform.Find("Running").gameObject.SetActive(false);
        
        activeAnimation = newAnimation;
        transform.Find(activeAnimation).gameObject.SetActive(true);
    }

    void DisableShooting() {
        isShooting = false;
    }

}

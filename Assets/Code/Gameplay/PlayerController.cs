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
    public int score = 0;
    public int projectileType = 0;
    float movementX = 0;
    float movementY = 0;
    bool collisionCooldown = false;
    bool shieldActive = false;
    string activeAnimation = "Idle";
    bool isShooting = false;
    int maxVines = 1;
    bool stickyVines = false;
    bool knockedFromLadder = false;

    Rigidbody2D rb;
    BoxCollider2D bc;
    Animator animator;
    SpriteRenderer[] spriteRenderers;
    [SerializeField] private LayerMask layerMask;
    public GameObject grapplePrefab;
    private int health = 3;
    [SerializeField] public bool playerHit {get; set;}
    // prevent gravityScale from turning back too soon:
    [SerializeField] private bool hitOffGroundOffset = false;
    [SerializeField] private float invincibilityDurationSeconds;
    [SerializeField] private float invincibilityDeltaTime = 0.15f;

    GameObject hud;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        animator = GetComponent<Animator>();

        hud = GameObject.Find("PlayerUI");
        hud.GetComponent<PlayerUI>().SetAmmo(ammoCount);
        SetActiveAnimation("Idle");
    }

    void Update() {
        if (!transform.root.Find("UI").Find("UIController").GetComponent<UIController>().paused && health >= 1) {
            if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) && ammoCount > 0 && IsGrounded()) {
                Attack();
            }

            if (knockedFromLadder) {
                canClimb = false;
                canClimbDown = false;
                knockedFromLadder = false;
            }
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("shooting") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && isShooting) {
            SetActiveAnimation("Idle");
            isShooting = false;
        }
        
    }

    void FixedUpdate() {
        if (!transform.root.Find("UI").Find("UIController").GetComponent<UIController>().paused && health >= 1) {
            // Horizontal movement:
            if (Input.GetAxisRaw("Horizontal") != 0 && !isShooting) {
                Walk();
            }

            // Can player climb and are they trying to climb:
            if (canClimb && Input.GetAxisRaw("Vertical") != 0 && !isShooting) {
                // Can player climb down, is the ladder below them and are they attempting to climb down:
                if (canClimbDown && currentLadderY < transform.localPosition.y && Input.GetAxisRaw("Vertical") < 0) {
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
            } else if (!canClimb && !playerHit || (IsGrounded() && !hitOffGroundOffset)) {
                rb.gravityScale = 1;
            }
        }

        Flip();
    }

    void Walk() {
        if (!playerHit || !hitOffGroundOffset) {
            movementX = Input.GetAxisRaw("Horizontal") * movementSpeed;
            transform.position += new Vector3(movementX, 0, 0) * Time.deltaTime;
        }
    }

    void Climb() {
        if (!playerHit || !hitOffGroundOffset) {
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
            movementY = Input.GetAxisRaw("Vertical") * climbingSpeed;
            transform.position += new Vector3(0, movementY, 0) * Time.deltaTime;
        }
    }

    void Attack() {
        GameObject grappleObject;
        switch (projectileType) {
            case 0:
                if (transform.parent.GetComponent<LevelManager>().CountVines() < maxVines && !isShooting) {
                    isShooting = true;
                    SetActiveAnimation("Shooting");
                    animator.SetTrigger("shot");
                    ChangeAmmoCount(-1);
                    grappleObject = Instantiate(grapplePrefab, new Vector3(transform.position.x, transform.position.y - (grapplePrefab.GetComponent<SpriteRenderer>().size.y/2), 0f), Quaternion.identity) as GameObject;
                    grappleObject.transform.parent = transform.parent;
                    grappleObject.GetComponent<Grapple>().stickyVines = stickyVines;
                } else if (stickyVines) {
                    transform.parent.GetComponent<LevelManager>().DestroyAllVines();
                }
                break;
            default:
                Debug.Log("Invalid projectile type");
                break;
        }
    }

    void ChangeAmmoCount(int amount) {
        ammoCount += amount;
        hud.GetComponent<PlayerUI>().SetAmmo(ammoCount);
    }

    void ChangeScore(int amount) {
        score += amount;
        hud.GetComponent<PlayerUI>().SetScore(score);
    }

    void OnTriggerEnter2D(Collider2D col) {

        if (col.gameObject.tag == "Ladder") {
            canClimb = true;
            canClimbDown = col.gameObject.transform.localPosition.y < transform.localPosition.y;
            currentLadderY = col.gameObject.transform.localPosition.y;
        }

        // Avoid double collisions:
        if (collisionCooldown) {
            return;
        }

        if (col.gameObject.tag == "Ball" && !playerHit) {
            HitPlayer(col.gameObject.transform.localPosition.x);
        }

        // Drops:
        if (col.gameObject.layer == 11) {
            HandleDrops(col.gameObject);
            Destroy(col.gameObject);
        }

        StartCoroutine(StartCollisionCooldown());

    }

    void HandleDrops(GameObject gameObject) {
        switch (gameObject.tag) {
            case "AmmoDrop":    if (ammoCount > 5) {
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
                                break;
            case "StickyVines": stickyVines = true;
                                maxVines = 1;
                                break;
        }
    }

    void ActiveShield() {
        // turn blue here or something
        shieldActive = true;
    }

    IEnumerator StartCollisionCooldown() {
        collisionCooldown = true;
        yield return new WaitForSeconds(0.1f);
        collisionCooldown = false;
    }
    
    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Ladder") {
            canClimb = false;
            canClimbDown = false;
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

        health--;
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
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(bc.bounds.center - bc.bounds.extents * 1.3f, bc.bounds.size * 0.05f, 0f, Vector2.down, extraHeight, layerMask);

        /* DEBUG:
        Color rayColor;
        if (raycastHit.collider != null) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        
        Debug.DrawRay(bc.bounds.center + new Vector3(bc.bounds.extents.x, 0), Vector2.down * (bc.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(bc.bounds.center - new Vector3(bc.bounds.extents.x, 0), Vector2.down * (bc.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(bc.bounds.center - new Vector3(bc.bounds.extents.x, bc.bounds.extents.y + extraHeight), Vector2.right * (bc.bounds.extents.x * 2), rayColor);
        */
        
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
                Debug.Log("hit ground");
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
        transform.Find("Idle").gameObject.SetActive(false);
        transform.Find("Hit").gameObject.SetActive(false);
        transform.Find("Shooting").gameObject.SetActive(false);
        
        activeAnimation = newAnimation;
        transform.Find(activeAnimation).gameObject.SetActive(true);
    }

}

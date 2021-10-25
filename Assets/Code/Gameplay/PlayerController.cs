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

    public int ammoCount = 10;
    public int score = 0;
    public int projectileType = 0;

    [Tooltip("Adjust starting height of spawned projectiles.")] public float projectileOffset;
    Rigidbody2D rb;
    BoxCollider2D bc;
    [SerializeField] private LayerMask layerMask;
    public GameObject grapplePrefab;
    private int health = 3;
    [SerializeField] public bool playerHit {get; set;}
    // prevent gravityScale from turning back too soon:
    [SerializeField] private bool hitOffGroundOffset = false;
    [SerializeField] private float invincibilityDurationSeconds;
    [SerializeField] private float invincibilityDeltaTime = 0.15f;

    public GameObject hud;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        ChangeAmmoCount(0);
        ChangeAmmoCount(0);
    }

    void Update() {
        if (!UIController.paused) {
            if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) && ammoCount > 0) {
                Attack();
            }

            if (playerHit) {
                canClimb = false;
                canClimbDown = false;
            }
        }
    }

    void FixedUpdate() {
        if (!UIController.paused) {
            // Horizontal movement:
            if (Input.GetAxisRaw("Horizontal") != 0) {
                Walk();
            }

            // Can player climb and are they trying to climb:
            if (canClimb && Input.GetAxisRaw("Vertical") != 0) {
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
                rb.gravityScale = 0;
                Climb();
            // If the player is unable to climb anymore, turn it's gravityScale back on:
            } else if (!canClimb && !playerHit || (IsGrounded() && !hitOffGroundOffset)) {
                rb.gravityScale = 1;
            }
        }
    }

    void Walk() {
        if (!playerHit || !hitOffGroundOffset) {
            float movementX = Input.GetAxisRaw("Horizontal") * movementSpeed;
            transform.position += new Vector3(movementX, 0, 0) * Time.deltaTime;
        }
    }

    void Climb() {
        float movementY = Input.GetAxisRaw("Vertical") * climbingSpeed;
        transform.position += new Vector3(0, movementY, 0) * Time.deltaTime;
    }

    void Attack() {
        ChangeAmmoCount(-1);
        switch (projectileType) {
            case 0:
                GameObject grappleObject = Instantiate(grapplePrefab, new Vector3(transform.position.x, transform.position.y + projectileOffset, 0f), Quaternion.identity) as GameObject;
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

        if (col.gameObject.tag == "Ball" && !playerHit) {
            HitPlayer(col.gameObject.transform.localPosition.x);
        }
        
        if (col.gameObject.tag == "Ladder" && !playerHit) {
            canClimb = true;
            canClimbDown = col.gameObject.transform.localPosition.y < transform.localPosition.y;
            currentLadderY = col.gameObject.transform.localPosition.y;
        }

        if (col.gameObject.tag == "AmmoDrop" && !playerHit) {
            ChangeAmmoCount(Random.Range(1, 4));
            Destroy(col.gameObject);
        }

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
        
        health--;
        int dir = enemyX < transform.localPosition.x ? 1 : -1;
        rb.gravityScale = 0.5f;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(2.5f * dir, 3.25f), ForceMode2D.Impulse);


        // Player dead:
        if (health <= 0) {
            Debug.Log("Player dead");
            GetComponentInChildren<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
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
            GetComponentInChildren<SpriteRenderer>().enabled = flash;
            flash = !flash;
            yield return new WaitForSeconds(invincibilityDeltaTime);
            if (IsGrounded()) {
                hitOffGroundOffset = false;
            }
        }

        GetComponentInChildren<SpriteRenderer>().enabled = true;
        playerHit = false;
    }
}

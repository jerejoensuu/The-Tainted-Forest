using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public float movementSpeed;
    public float gravity;
    public bool canClimb = false;
    public float climbingSpeed;

    public int ammoCount = 10;
    public int projectileType = 0;

    [Tooltip("Adjust starting height of spawned projectiles.")]
    public float projectileOffset;
    Rigidbody2D rb;
    TextMeshPro ammoText;
    public GameObject grapplePrefab;
    private int health = 3;
    private bool playerHit = false;
    
    void Start() {
        SemisolidPlatform.playerObjects.Add(this.gameObject);
        rb = GetComponent<Rigidbody2D>();
        ammoText = GetComponentInChildren<TextMeshPro>();
        ammoText.text = ammoCount.ToString();
    }

    void Update() {
        if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Jump")) && ammoCount > 0) {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FixedUpdate() {
        if (Input.GetAxisRaw("Horizontal") != 0) {
            Walk();
        }
        if (canClimb == true && Input.GetAxisRaw("Vertical") != 0) {
            Climb();
        }
    }

    void Walk() {
        float movementX = Input.GetAxisRaw("Horizontal") * movementSpeed;

        //rb.MovePosition(transform.position + new Vector3(movementX, 0, 0) * Time.deltaTime);
        transform.position += new Vector3(movementX, 0, 0) * Time.deltaTime;
    }

    void Climb() {
        float movementY = Input.GetAxisRaw("Vertical") * climbingSpeed;
        transform.position += new Vector3(0, movementY, 0) * Time.deltaTime;
    }

    void Attack() {
        ammoCount--;
        ammoText.text = ammoCount.ToString();
        switch (projectileType) {
            case 0:
                GameObject grappleObject = Instantiate(grapplePrefab, new Vector3(transform.position.x, transform.position.y + projectileOffset, 0f), Quaternion.identity) as GameObject;
                break;
            default:
                Debug.Log("Invalid projectile type");
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D col) {

        if (col.gameObject.tag == "Ball" && !playerHit) {
            HitPlayer();
        }
        
        if (col.gameObject.tag == "Ladder") {
            canClimb = true;
            rb.gravityScale = 0;
        }

    }
    
    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Ladder") {
            canClimb = false;
            rb.gravityScale = 1f;
        }
    }

    void HitPlayer() {
        Debug.Log("Player hit");
        Debug.Log("Health: " + health);
        health--;

        playerHit = true;
        rb.AddForce(new Vector2(50, 50));

        // Player dead:
        if (health <= 0) {
            Debug.Log("Player dead");
            GetComponentInChildren<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public float movementSpeed;
    // public float gravity;

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
        Movement(Input.GetAxisRaw("Horizontal"));
    }

    void Movement(float movementDirection) {
        Vector3 movement = new Vector3(movementDirection * movementSpeed, 0, 0);

        //rb.MovePosition(transform.position + movement * Time.deltaTime);
        rb.velocity = movement * Time.deltaTime;
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

    }

    void HitPlayer() {
        Debug.Log("Player hit");
        Debug.Log("Health: " + health);
        health--;

        playerHit = true;
        rb.gravityScale = 1f;
        rb.AddForce(new Vector2(50, 50));

        // Player dead:
        if (health <= 0) {
            Debug.Log("Player dead");
            GetComponentInChildren<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
    }
}

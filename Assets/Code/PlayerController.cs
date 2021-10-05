using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
    public float movementSpeed;
    public float gravity;
    public bool canClimb = false;
    public bool climbing = false;
    public float climbingSpeed;

    public int ammoCount = 10;
    public int projectileType = 0;

    [Tooltip("Adjust starting height of spawned projectiles.")]
    public float projectileOffset;
    Rigidbody2D rb;
    TextMeshPro ammoText;
    public GameObject grapplePrefab;

    void Awake() {
        SemisolidPlatform.playerObjects.Add(this.gameObject);
    }
    
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
        Vector3 movement = new Vector3(0f, 0f, 0f);
        if (canClimb && Input.GetAxis("Vertical") != 0) {
            climbing = true;
        }
        if (climbing) {
            movement += Climbing();
        }
        else {
            movement += new Vector3(0f, -gravity, 0f);
        }

        movement += Movement();
        rb.MovePosition(transform.position + movement * Time.deltaTime);
    }

    Vector3 Movement() {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal") * movementSpeed, 0f, 0f);

        //rb.MovePosition(transform.position + movement * Time.deltaTime);

        return move;
    }

    Vector3 Climbing() {
        Vector3 climb = new Vector3(0, Input.GetAxis("Vertical") * climbingSpeed, 0);

        return climb;
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
        if (col.gameObject.tag == "Ladder") {
            canClimb = true;
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.tag == "Ladder") {
            canClimb = false;
            climbing = false;
        }
    }
}

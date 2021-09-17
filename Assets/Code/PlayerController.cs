using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float movementSpeed;
    // public float gravity;
    public int projectileType = 0;
    public float projectileOffset;
    Rigidbody2D rb;

    public GameObject grapplePrefab;
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Attack();
        }
    }

    void FixedUpdate() {
        Movement(Input.GetAxis("Horizontal"));
    }

    void Movement(float movementDirection) {
        Vector3 movement = new Vector3(movementDirection * movementSpeed, 0, 0);

        rb.MovePosition(transform.position + movement * Time.deltaTime);
    }

    void Attack() {
        switch (projectileType) {
            case 0:
                GameObject grappleObject = Instantiate(grapplePrefab, new Vector3(transform.position.x, transform.position.y + projectileOffset, 0f), Quaternion.identity) as GameObject;
                break;
            default:
                Debug.Log("No projectile type");
                break;
        }
    }
}

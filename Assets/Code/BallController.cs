using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float moveSpeed;
    public float gravity = 0.05f;
    float direction = 1;
    public float size;
    public float spawnSizeMultiplier = 0.5f;
    public float minimumSize;
    float momentum = 0;
    public GameObject debugDot;
    public GameObject circlePrefab;


    // Start is called before the first frame update
    void Start() {
        transform.localScale = new Vector3(size, size, 1);
        Debug.Log("Size: " + size);
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        //GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(direction, momentum , 0) * moveSpeed * Time.deltaTime);
        GetComponent<Rigidbody2D>().velocity = new Vector3(direction * moveSpeed, momentum , 0) * Time.deltaTime;
        momentum -= gravity;

        if (transform.position.x > 11 || transform.position.x < -11) {
            Destroy(gameObject);
            Debug.Log("BALL OFF-SCREEN");
        }

    }

    void Update() {

        if (Input.GetMouseButtonDown(0)) {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            
            if (hit && hit.collider.gameObject.transform.position == transform.position) {
                    DestroyBall();
            }
        }   
    }

    void DestroyBall() {
        if (size >= minimumSize) {
            Debug.Log(size);
            SpawnBalls(-1, size * spawnSizeMultiplier);
            SpawnBalls(1, size * spawnSizeMultiplier);
        }
        Destroy(gameObject);
    }

    void SpawnBalls(float direction, float newSize) {
        GameObject newBall = Instantiate(circlePrefab, transform.position, Quaternion.identity) as GameObject;

        newBall.GetComponent<BallController>().direction = direction;
        newBall.GetComponent<BallController>().momentum = gravity * 33;
        newBall.GetComponent<BallController>().size = newSize;
    }

    void OnTriggerEnter2D(Collider2D col) {

        // Detecting collision with player
        if (col.gameObject.layer == LayerMask.NameToLayer("PlayerTrigger")) {
            Debug.Log("Player is dead");
        }

        // Detecting collision with player projectiles
        else if (col.gameObject.tag == "Projectile") {
            Destroy(col.gameObject);
            DestroyBall();
        }
    }
    void OnCollisionEnter2D(Collision2D col) {

        //Debug.Log("collider:" + col.collider.GetType());
        //Debug.Log("otherCollider:" + col.otherCollider.GetType());
        if (col.collider.tag == "Wall") {
            Vector2 contactP = col.GetContact(0).point;

            float deltaX = col.GetContact(0).otherCollider.transform.position.x - contactP.x;
            float deltaY = col.GetContact(0).otherCollider.transform.position.y - contactP.y;
            //Debug.Log("deltaX:" + deltaX);
            //Debug.Log("deltaY:" + deltaY);


            Instantiate(debugDot, contactP, Quaternion.identity);

            if (Mathf.Abs(deltaX) < Mathf.Abs(deltaY)) {
                momentum += gravity; //counter gravity's effect during the frame before colliding to stop the ball's momentum increasing
                momentum *= -1;
                // Debug.Log("hit floor or ceiling");
            } else if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                if (deltaX > 0) {
                    direction = 1;
                    // Debug.Log("hit left wall");
                } else {
                    direction = -1;
                    // Debug.Log("hit right wall");
                }
            }
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float moveSpeed;
    public float gravity = 0.05f;
    float direction = 1;
    public float size;
    [Tooltip("Ball spawn size percentage.")]
    public float spawnSizeMultiplier = 0.5f;
    [Tooltip("Spawn new balls only if current size >= this.")]
    public float minimumSize;
    float momentum = 0;
    float lastMomentum = 0;
    float lastY = 0;
    float maxY = 0;
    int stationaryYCounter = 0;
    public GameObject debugDot;
    public GameObject circlePrefab;


    // Start is called before the first frame update
    void Start() {
        transform.localScale = new Vector3(size, size, 1);
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        GetComponent<Rigidbody2D>().velocity = new Vector3(direction * moveSpeed, momentum , 0) * Time.deltaTime;
        lastMomentum = momentum;
        momentum -= gravity;
        
        // Reset momentum if a ball is detected moving on a flat surface (ie. not bouncing)
        if (lastY == gameObject.transform.localPosition.y) {
            stationaryYCounter++;
            if (stationaryYCounter > 3) {
                momentum = 0;
                stationaryYCounter = 0;
                //Debug.Log("Flat surface detected");
            }
        } else {
            stationaryYCounter = 0;
        }
        lastY = gameObject.transform.localPosition.y;

        //debug:
        /*
        if (gameObject.transform.localPosition.y > maxY) {
            maxY = gameObject.transform.localPosition.y;
            Debug.Log("Highest Y achieved: " + maxY);
        }
        */

        // Destroy off-screen balloons
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

        // Collision with player
        if (col.gameObject.layer == LayerMask.NameToLayer("PlayerTrigger")) {
            //Debug.Log("Player is dead");
            col.gameObject.transform.parent.GetComponentInChildren<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }

        // Collision with player projectiles
        if (col.gameObject.tag == "Projectile") {
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

            Instantiate(debugDot, contactP, Quaternion.identity); //place debugDot to show collision point

            if (Mathf.Abs(deltaX) < Mathf.Abs(deltaY)) {
                if (deltaY > 0) {
                    momentum = Mathf.Abs(lastMomentum + gravity);
                    Debug.Log("hit floor");
                } else {
                    momentum = Mathf.Abs(lastMomentum + gravity) * -1;
                    Debug.Log("hit ceiling");
                }
            } else  {
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

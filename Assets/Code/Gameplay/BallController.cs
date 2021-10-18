using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float moveSpeed;
    public float gravity = 0.05f;
    [Tooltip("-1 and 1 for left and right, 0 for random direction.")]
    [SerializeField] float direction;
    public float size;
    [Tooltip("Ball spawn size percentage.")]
    public float spawnSizeMultiplier = 0.5f;
    [Tooltip("Spawn new balls only if current size >= this.")]
    public float minimumSize;
    bool isDestroyed = false;
    float momentum = 0;
    float lastMomentum = 0;
    float lastY = 0;
    int stationaryYCounter = 0;
    public GameObject debugDot;
    public GameObject circlePrefab;


    // Start is called before the first frame update
    void Start() {
        transform.localScale = new Vector3(size, size, 1);

        if (direction == 0) {
            direction = Mathf.Sign(Random.Range(-1, 1)); // random direction
        } else {
            direction = Mathf.Sign(direction); // correct for inputs <-1 and >1
        }
        
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
            }
        } else {
            stationaryYCounter = 0;
        }
        lastY = gameObject.transform.localPosition.y;

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
        if (!isDestroyed) {
            isDestroyed = true;
            if (size >= minimumSize) {
                SpawnBalls(-1, size * spawnSizeMultiplier);
                SpawnBalls(1, size * spawnSizeMultiplier);
            }
            Destroy(gameObject);
        }
    }

    void SpawnBalls(float direction, float newSize) {
        GameObject newBall = Instantiate(circlePrefab, transform.position, Quaternion.identity) as GameObject;

        newBall.GetComponent<BallController>().direction = direction;
        newBall.GetComponent<BallController>().momentum = gravity * 33;
        newBall.GetComponent<BallController>().size = newSize;
    }

    void OnTriggerEnter2D(Collider2D col) {
        // Collision with player projectiles
        if (col.gameObject.tag == "Projectile") {
            DestroyBall();
        }
    }
    void OnCollisionEnter2D(Collision2D col) {

        if (col.collider.tag == "Wall") {
            Vector2 contactP = col.GetContact(0).point;

            float deltaX = col.GetContact(0).otherCollider.transform.position.x - contactP.x;
            float deltaY = col.GetContact(0).otherCollider.transform.position.y - contactP.y;

            //place debugDot to show collision point
            //Instantiate(debugDot, contactP, Quaternion.identity);

            if (Mathf.Abs(deltaX) < Mathf.Abs(deltaY)) {
                if (deltaY > 0) {
                    momentum = Mathf.Abs(lastMomentum + gravity);
                } else {
                    momentum = Mathf.Abs(lastMomentum + gravity) * -1;
                }
            } else  {
                if (deltaX > 0) {
                    direction = 1;
                } else {
                    direction = -1;
                }
            }
        }

    }

}

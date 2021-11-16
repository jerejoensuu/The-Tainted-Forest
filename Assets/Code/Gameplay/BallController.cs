using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float moveSpeed;
    [Range(1, 4)] [SerializeField] [Tooltip("0.4, 0.7, 1.3, 2.25")] public int size;
    private float freezeFactor = 1;
    public float gravity = 0.05f;
    [Tooltip("-1 and 1 for left and right, 0 for random direction.")]
    [SerializeField] float direction;
    [Tooltip("Ball spawn size percentage.")]
    //bool isDestroyed = false;
    float momentum = 0;
    float lastMomentum = 0;
    float lastY = 0;
    int stationaryYCounter = 0;
    public GameObject circlePrefab;
    private SpriteRenderer sr;

    private List<float> sizes = new List<float>{ 0.4f, 0.7f, 1.3f, 2.25f };

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start() {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = GetSize(size);

        if (direction == 0) {
            direction = Mathf.Sign(Random.Range(-1, 1)); // random direction
        } else {
            direction = Mathf.Sign(direction); // correct for inputs <-1 and >1
        }
        
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (levelManager != null) { levelManager.bubblesRemaining.Add(this.gameObject); }
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        GetComponent<Rigidbody2D>().velocity = new Vector3(direction * moveSpeed, momentum, 0) * freezeFactor * Time.deltaTime;
        if (freezeFactor == 1) {
            lastMomentum = momentum;
        }
        momentum -= gravity * freezeFactor;
        
        // Reset momentum if a ball is detected moving on a flat surface (ie. not bouncing)
        if (lastY == gameObject.transform.localPosition.y && freezeFactor == 1) {
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

    Vector2 GetSize(int s) {
        return new Vector2(sizes[s-1], sizes[s-1]);
    }


    public void Freeze() {
        // WIP:
        // for (float i = 0; i < 5; i += 0.01f) {
        //     yield return new WaitForSeconds(0.01f);
        //     freezeFactor *= 0.97f;
        // }

        // for (float i = 0; i < 1; i += 0.03f) {
        //     yield return new WaitForSeconds(0.01f);
        //     freezeFactor = i;
        // }

        // freezeFactor = 1;

        freezeFactor = 0;
        sr.color = Color.blue;        
    }

    public void UnFreeze() {
        sr.color = Color.white;
        freezeFactor = 1;
    }

    bool isDestroyed = false;
    public void DestroyBall() {
        if (!isDestroyed) {
            isDestroyed = true;
            if (size > 1) {
                SpawnBalls(-1, size - 1);
                SpawnBalls(1, size - 1);
            }
            if (levelManager != null) {
                levelManager.bubblesRemaining.Remove(this.gameObject);
                levelManager.CheckRemainingBubbles();
            }
            GetComponentInChildren<BallDestroyAudio>().PlaySound();
            Destroy(gameObject);
        }
    }

    void SpawnBalls(float direction, int newSize) {
        GameObject newBall = Instantiate (circlePrefab,
                                            new Vector2(transform.position.x + (sizes[newSize] * 0.25f) * direction, transform.position.y),
                                            Quaternion.identity) as GameObject;
        newBall.transform.parent = transform.parent;

        newBall.GetComponent<BallController>().direction = direction;
        newBall.GetComponent<BallController>().momentum = gravity * 33;
        newBall.GetComponent<BallController>().size = newSize;
    }

    void OnTriggerEnter2D(Collider2D col) {
        // Collision with player projectiles
        if (col.gameObject.layer == 14) {
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

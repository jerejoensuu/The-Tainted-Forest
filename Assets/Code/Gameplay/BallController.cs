using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    [Range(1, 4)] [SerializeField] [Tooltip("0.4, 0.7, 1.3, 2.25")] public int size;
    [Tooltip("-1 and 1 for left and right, 0 for random direction.")] [Range(-1, 1)] [SerializeField] int direction;
    private int pointsAward = 100;

    public float moveSpeed;
    float ogSpeed;
    private float freezeFactor = 1;
    public float gravity = 0.05f;
    float momentum = 0;
    float lastMomentum = 0;
    float lastY = 0;
    float lastX = 0;
    int stationaryYCounter = 0;
    int stationaryXCounter = 0;
    public GameObject circlePrefab;
    private SpriteRenderer sr;

    // Bounce compression:
    float maxYCompression = 0.7f;
    public Vector2 contactPY;
    public float yCompression;
    public bool yCompressing = false;
    public bool yCompressed = false;
    public bool yDecompressing = false;
    float maxXCompression = 0.85f;
    Vector2 contactPX;
    float xCompression;
    bool xCompressing = false;
    bool xCompressed = false;
    bool xDecompressing = false;

    private List<float> sizes = new List<float>{ 0.4f, 0.7f, 1.3f, 2.25f };

    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start() {
        ogSpeed = moveSpeed;
        xCompression = GetSize(size).x;
        yCompression = GetSize(size).y;

        try {
            bool test = transform.parent.name != "LevelManager";
        }
        catch (System.Exception) {
            Debug.Log("OBJECT NOT SET AS CHILD OF LevelManager!");
            throw;
        }

        sr = GetComponent<SpriteRenderer>();
        transform.localScale = GetSize(size);

        if (direction == 0) {
            direction = (int)Mathf.Sign(Random.Range(-1, 1)); // random direction
        }
        
        levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (levelManager != null) { levelManager.bubblesRemaining.Add(this.gameObject); }
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        if (!isDestroyed) {
            GetComponent<Rigidbody2D>().velocity = new Vector3(direction * moveSpeed, momentum, 0) * freezeFactor * Time.deltaTime;
        } else {
            GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
            GetComponent<Collider2D>().enabled = false;
        }
        if (freezeFactor == 1) {
            lastMomentum = momentum;
        }
        momentum -= gravity * freezeFactor;

        CheckIfNotBouncing();

        if (yCompressing) {
            Compress("y");
        } else if (yDecompressing) {
            Decompress("y");
        }
        if (xCompressing) {
            Compress("x");
        } else if (xDecompressing) {
            Decompress("x");
        }

        // Destroy off-screen balloons
        if (transform.position.x > 11 || transform.position.x < -11) {
            Destroy(gameObject);
            Debug.Log("BALL OFF-SCREEN");
        }

    }

    Vector2 GetSize(int s) {
        return new Vector2(sizes[s-1], sizes[s-1]);
    }

    void CheckIfNotBouncing() {
        // Reset momentum if a ball is detected moving on a flat surface (ie. not bouncing)
        if (Mathf.Abs(lastY - gameObject.transform.localPosition.y) < 0.0001f && freezeFactor == 1) {
            stationaryYCounter++;
            if (stationaryYCounter > 3) {
                momentum = 0;
                stationaryYCounter = 0;
            }

        } else {
            stationaryYCounter = 0;
        }
        if (GetComponent<Rigidbody2D>().velocity.y > 0.1f) {
            moveSpeed = ogSpeed;
        }

        // Set horizontal speed to 0 if stuck
        if (Mathf.Abs(lastX - gameObject.transform.localPosition.x) < 0.001f && freezeFactor == 1) {
            stationaryXCounter++;
            if (stationaryXCounter > 3) {
                moveSpeed = 0;
                stationaryXCounter = 0;
            }

        } else {
            stationaryXCounter = 0;
        }

        lastY = gameObject.transform.localPosition.y;
        lastX = gameObject.transform.localPosition.x;
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
    }

    public void UnFreeze() {
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
                AddToScore();
            }
            GetComponentInChildren<BallDestroyAudio>().PlaySound();
            GetComponent<Animator>().SetTrigger("Burst");
        }
    }

    void PopBubble() {
        Destroy(gameObject);
    }

    public void AddToScore() {
        int points = transform.root.Find("Player").GetComponent<PlayerController>().combo * pointsAward;
        transform.root.Find("UI/Canvas/PopupTextManager").GetComponent<PopupTextManager>().NewPopupText("+" + (points).ToString(), transform.position);
        GameObject.Find("PlayerUI").GetComponent<PlayerUI>().ChangeScore(points);
    }

    void SpawnBalls(int direction, int newSize) {
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
            if (Random.Range(0f, 1f) < 0.2f) {
                GameObject drop = Instantiate(transform.root.Find("PlatformAndDropManager").GetComponent<DropManager>().GetRandomDrop(), transform.position, Quaternion.identity) as GameObject;
                drop.transform.parent = transform.root.transform;
            }
            transform.root.Find("Player").GetComponent<PlayerController>().combo = transform.root.Find("Player").GetComponent<PlayerController>().combo + 1;
            Destroy(col.gameObject);
            DestroyBall();
        }
        
        if (col.tag == "Wall" || col.tag == "BreakableWall") {
            float deltaX = transform.position.x - col.ClosestPoint(transform.position).x;
            float deltaY = transform.position.y - col.ClosestPoint(transform.position).y;

            if (Mathf.Abs(deltaX) < Mathf.Abs(deltaY) && !yDecompressing) {
                if (Mathf.Sign(deltaY) != Mathf.Sign(momentum)) {
                    maxYCompression = 1 - Mathf.Abs(momentum / 10000) * 1.3f;
                    yCompressing = true;
                }
                contactPY = col.ClosestPoint(transform.position);
                Bounce("y");
            } else if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY) && !xDecompressing) {
                if (Mathf.Sign(deltaX) != Mathf.Sign(direction)) {
                    xCompressing = true;
                }
                contactPX = col.ClosestPoint(transform.position);
                Bounce("x");
            }
            
        }
    }

    void Bounce(string axis) {

        //place debugDot to show collision point
        //Instantiate(debugDot, contactP, Quaternion.identity);

        if (axis == "y") {

            if (!yCompressing) {
                if (momentum < 0) {
                    momentum = Mathf.Abs(lastMomentum);
                } else {
                    momentum = Mathf.Abs(lastMomentum) * -1;
                }
            } else if (yCompressing) {
                Compress("y");
            }

        } else if (axis == "x") {

            if (!xCompressing) {
                if (contactPX.x < transform.position.x) {
                    direction = 1;
                } else {
                    direction = -1;
                }
            } else if (xCompressing) {
                Compress("x");
            }

        }
        
    }

    void Compress(string axis) {

        if (axis == "y") {
            yCompression = Mathf.Abs(GetComponent<Collider2D>().bounds.center.y - contactPY.y) * 2;
        } else {
            xCompression = Mathf.Abs(GetComponent<Collider2D>().bounds.center.x - contactPX.x) * 2;
        }

        if (yCompression / GetSize(size).y <= maxYCompression && yCompressing) {
            yCompression = GetSize(size).y * maxYCompression;
            yCompressing = false;
            yCompressed = true;
            yDecompressing = true;
        }

        if (xCompression / GetSize(size).x <= maxXCompression && xCompressing) {
            xCompression = GetSize(size).x * maxXCompression;
            xCompressing = false;
            xCompressed = true;
            xDecompressing = true;
            Bounce("x");
        }

        transform.localScale = new Vector3(xCompression, yCompression);

    }

    void Decompress(string axis) {
        if (yCompressed) {
            yCompressed = false;
            Bounce("y");
        }
        if (xCompressed) {
            xCompressed = false;
            Bounce("x");
        }

        if (axis == "y") {
            yCompression = Mathf.Abs(GetComponent<Collider2D>().bounds.center.y - contactPY.y) * 2;
        } else {
            xCompression = Mathf.Abs(GetComponent<Collider2D>().bounds.center.x - contactPX.x) * 2;
        }

        //float distance = Mathf.Abs(Vector3.Distance(new Vector3(contactPY.x, contactPY.y), GetComponent<Collider2D>().bounds.center));
        float distance = Mathf.Abs(GetComponent<Collider2D>().bounds.center.y - contactPY.y) * 1.9f;
        if (yCompression >= GetSize(size).y && yDecompressing && distance >= GetSize(size).y) {
            yCompression = GetSize(size).y;
            yDecompressing = false;
        }

        if (xCompression >= GetSize(size).x && xDecompressing) {
            xCompression = GetSize(size).x;
            xDecompressing = false;
        }

        transform.localScale = new Vector3(xCompression, yCompression);

    }

}

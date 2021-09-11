using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float moveSpeed;
    public float gravity = 0.05f;
    float direction = 1;
    public float size;
    float momentum = 0;
    public GameObject debugDot;
    public GameObject Circle;


    // Start is called before the first frame update
    void Start() {
        transform.localScale = new Vector3(size,size,1);
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(direction, momentum , 0) * moveSpeed * Time.deltaTime);
        momentum -= gravity;

        if (transform.position.x > 10 || transform.position.x < -10) {
            Destroy(gameObject);
        }

    }

    void Update() {

        if (Input.GetMouseButtonDown(0)) {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
 
            if(hit.collider.gameObject.transform.position == transform.position) {
                SpawnBalls(-1, size * 0.25f);
                SpawnBalls(1, size * 0.25f);
                //Destroy(gameObject);
            }
            
        }
            
    }

    public GameObject SpawnBalls(float direction, float size) {
        GameObject newBall = Instantiate( Circle, transform.position, Quaternion.identity) as GameObject;

        newBall.GetComponent<BallController>().direction = direction;
        newBall.GetComponent<BallController>().momentum = 1.5f;
        newBall.GetComponent<BallController>().size  = size;

        return newBall;
    }

    void OnCollisionEnter2D(Collision2D col) {

        //Debug.Log("collider:" + col.collider.GetType());
        //Debug.Log("otherCollider:" + col.otherCollider.GetType());
        if (col.collider.GetType() != col.otherCollider.GetType()) {
            Vector2 contactP = col.GetContact(0).point;

            float deltaX = col.GetContact(0).otherCollider.transform.position.x - contactP.x;
            float deltaY = col.GetContact(0).otherCollider.transform.position.y - contactP.y;
            //Debug.Log("deltaX:" + deltaX);
            //Debug.Log("deltaY:" + deltaY);


            Instantiate(debugDot, contactP, Quaternion.identity);

            if (Mathf.Abs(deltaX) < Mathf.Abs(deltaY)) {
                momentum += gravity; //counter gravity's effect during the frame before colliding to stop the ball's momentum increasing
                momentum *= -1;
                //Debug.Log("hit floor or ceiling");
            } else if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
                if (deltaX > 0) {
                    direction = 1;
                    //Debug.Log("hit left wall");
                } else {
                    direction = -1;
                    //Debug.Log("hit right wall");
                }
            }
        }

    }

}

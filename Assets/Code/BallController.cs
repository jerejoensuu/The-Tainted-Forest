using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float moveSpeed;
    public float gravity = 0.05f;
    float direction = 1;
    float momentum = 0;
    public GameObject debugDot;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        //transform.Translate(new Vector3(direction, 0 , 0) * moveSpeed * Time.deltaTime);
        GetComponent<Rigidbody2D>().MovePosition(transform.position + new Vector3(direction, momentum , 0) * moveSpeed * Time.deltaTime);
    
        momentum -= gravity;
    }

    void OnCollisionEnter2D(Collision2D col) {
        Vector2 contactP = col.GetContact(0).point;

        float deltaX = col.GetContact(0).otherCollider.transform.position.x - contactP.x;
        float deltaY = col.GetContact(0).otherCollider.transform.position.y - contactP.y;
        Debug.Log("deltaX:" + deltaX);
        Debug.Log("deltaY:" + deltaY);


        Instantiate(debugDot, contactP, Quaternion.identity);

        if (Mathf.Abs(deltaX) < Mathf.Abs(deltaY)) {
            //momentum = Mathf.Abs(momentum);
            momentum *= -1;
            Debug.Log("hit floor or ceiling");
        } else if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY)) {
            if (deltaX > 0) {
                direction = 1;
                Debug.Log("hit left wall");
            } else {
                direction = -1;
                Debug.Log("hit right wall");
            }
        }

    }
}

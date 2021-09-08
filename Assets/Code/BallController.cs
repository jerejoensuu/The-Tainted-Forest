using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{

    public float moveSpeed;
    BoxCollider2D bounceHitbox;
    List<BoxCollider2D> walls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.Translate(new Vector3(1, 0 , 0) * moveSpeed * Time.deltaTime);

    

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("OnCollisionEnter2D");
    }
}

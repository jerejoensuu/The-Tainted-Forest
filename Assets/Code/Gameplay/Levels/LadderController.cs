using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour {
    
    

    void Start() {
        
        transform.Find("OneWayTop").GetComponent<BoxCollider2D>().offset = new Vector2(0, GetComponent<SpriteRenderer>().size.y/2 - transform.Find("OneWayTop").GetComponent<BoxCollider2D>().size.y/2);
        GetComponent<BoxCollider2D>().size = new Vector2(0.3f, GetComponent<SpriteRenderer>().size.y);
        GetComponent<BoxCollider2D>().offset = new Vector2(0f, 0.1f/2);

    }

}

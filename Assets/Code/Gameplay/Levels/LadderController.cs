using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour {
    
    SpriteRenderer sp;
    BoxCollider2D bc;
    BoxCollider2D oneWay_bc;

    void Start() {
        sp = GetComponent<SpriteRenderer>();
        bc = GetComponent<BoxCollider2D>();
        oneWay_bc = transform.Find("OneWayTop").GetComponent<BoxCollider2D>();
        
        oneWay_bc.offset = new Vector2(0, sp.size.y/2 - oneWay_bc.size.y/2);
        bc.size = new Vector2(0.3f, sp.size.y);
        bc.offset = new Vector2(0f, 0.1f/2);

    }

}

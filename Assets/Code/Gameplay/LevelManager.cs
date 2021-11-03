using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    
    void Start() {
        
    }

    public void FreezeBubbles() {
        foreach (Transform child in transform) {
            if (child.tag == "Ball") {
                StartCoroutine(child.GetComponent<BallController>().FreezeBall());
            }
        }
    }

    public void DamageAllBubbles() {
        List<GameObject> bubbles = new List<GameObject>();
        foreach (Transform child in transform) {
            if (child.tag == "Ball") {
                bubbles.Add(child.gameObject);
            }
        }
        foreach (GameObject bubble in bubbles) {
            bubble.GetComponent<BallController>().DestroyBall();
        }
    }
}

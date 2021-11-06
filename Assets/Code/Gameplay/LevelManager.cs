using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    
    void Awake() {
        int bubbleCount = 0;
        foreach (Transform child in transform) {
            if (child.tag == "Ball") {
                bubbleCount += (int)Mathf.Pow(child.GetComponent<BallController>().size, 2);
            }
        }

        transform.Find("Player").GetComponent<PlayerController>().ammoCount = (int)(bubbleCount * 1.3f);
    }

    public IEnumerator FreezeBubbles() {

        for (float i = 0; i < 5; i += 0.01f) {
            foreach (Transform child in transform) {
                if (child.tag == "Ball") {
                    child.GetComponent<BallController>().Freeze();
                }
            }
            yield return new WaitForSeconds(0.01f);
        }

        foreach (Transform child in transform) {
            if (child.tag == "Ball") {
                child.GetComponent<BallController>().UnFreeze();
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

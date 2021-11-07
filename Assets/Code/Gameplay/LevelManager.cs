using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public List<GameObject> bubblesRemaining = new List<GameObject>();
    private bool levelWon = false;
    private bool levelLost = false;
    
    void Awake() {
        int bubbleCount = 0;
        foreach (Transform child in transform) {
            if (child.tag == "Ball") {
                bubbleCount += (int)Mathf.Pow(child.GetComponent<BallController>().size, 2);
            }
        }

        transform.Find("Player").GetComponent<PlayerController>().ammoCount = (int)(bubbleCount * 1.3f);
    }

    public void CheckRemainingBubbles() {
        StartCoroutine(ICheckRemainingBubbles());
    }

    IEnumerator ICheckRemainingBubbles() { // Delay the check by small amount because spawning seems to be slow!
        float delay = 0.5f;

        yield return new WaitForSeconds(delay);

        if (bubblesRemaining.Count == 0) {
            LevelWin();
        }
    }

    public void LevelWin() {
        if (!levelWon && !levelLost) {
            levelWon = true;
            GameObject.Find("UIController").GetComponent<UIController>().LevelWin();
        }
    }

    public void LevelLose() {
        if (!levelWon && !levelLost) {
            levelLost = true;
            GameObject.Find("UIController").GetComponent<UIController>().LevelLose();
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public List<GameObject> bubblesRemaining = new List<GameObject>();
    private bool levelWon = false;
    private bool levelLost = false;
    [Range(1, 2)] [SerializeField] public int theme = 1;
    [Range(1, 3)] [SerializeField] int taintLevel = 1;
    
    void Awake() {
        int bubbleCount = 0;
        foreach (Transform child in transform) {

            if (child.tag == "Ball") {
                bubbleCount += (int)Mathf.Pow(child.GetComponent<BallController>().size, 2);
            }
            
        }

        transform.Find("PlatformAndDropManager").GetComponent<DropManager>().ApplyTheme(theme);
        ApplyBackground();

        transform.Find("Player").GetComponent<PlayerController>().ammoCount = (int)(bubbleCount * 1.2f);


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

    public int CountVines() { 
        int bubbleCount = 0;
        foreach (Transform child in transform) {
            if (child.tag == "Vine") {
                bubbleCount++;
            }
        }
        return bubbleCount;
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

    public void DestroyAllVines() {
        foreach (Transform child in transform) {
            if (child.tag == "Vine") {
                Debug.Log("Destroyed");
                Destroy(child.gameObject);
            }
        }
    }

    private void ApplyBackground() {
        transform.Find("Backgrounds").GetChild(theme-1).gameObject.SetActive(false);
        transform.Find("Backgrounds").GetChild(0).GetChild(0).gameObject.SetActive(false);

        transform.Find("Backgrounds").GetChild(theme-1).gameObject.SetActive(true);
        transform.Find("Backgrounds").GetChild(theme-1).GetChild(taintLevel-1).gameObject.SetActive(true);
    }
}

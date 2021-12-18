using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TaintedForest;

public class LevelManager : MonoBehaviour {

    public List<GameObject> bubblesRemaining = new List<GameObject>();

    public bool levelWon = false;
    public bool levelLost = false;
    [Range(1, 2)] [SerializeField] public int theme = 1;
    [Range(1, 3)] [SerializeField] public int taintLevel = 1;
    [Range(10, 180)] [SerializeField] [Tooltip("In seconds")] public int time = 90;
    public GameObject blackScreen;
    public GameObject freezeEffect;
    public AudioSource audioSrc;
    
    void Awake() {
        audioSrc = GetComponent<AudioSource>();
        StartCoroutine(Transition());
        int bubbleCount = 0;
        foreach (Transform child in transform) {

            if (child.tag == "Ball") {
                bubbleCount += (int)Mathf.Pow(child.GetComponent<BallController>().size, 2);
            }
            
        }

        transform.Find("PlatformAndDropManager").GetComponent<DropManager>().ApplyTheme(theme);
        ApplyBackground();

        transform.Find("Player").GetComponent<PlayerController>().ammoCount = (int)(bubbleCount * 1.2f);
        StartCoroutine(CheckRemainingBubbles());

    }

    IEnumerator CheckRemainingBubbles() {
        float delay = 0.5f;
        bool loop = true;

        while (loop) {
            loop = false;
            foreach (Transform child in transform) {
                if (child.tag == "Ball") {
                    loop = true;
                }
            }

            yield return new WaitForSeconds(delay);
        }

        LevelWin();
        
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

    public bool SubmitScore() {
        Score scores = new Score(GameData.GetFilePath());
        if (scores.Add(int.Parse(SceneManager.GetActiveScene().name) - 1, GameObject.Find("PlayerUI").GetComponent<PlayerUI>().GetScore())) {
            scores.Save();
            return true;
        }
        return false;
    }

    public bool FindDrops(GameObject drop) {
        foreach (Transform child in transform) {
            if (child.tag == drop.tag) {
                return true;
            }
        }
        return false;
    }

    public void LevelWin() {
        if (!levelWon && !levelLost) {
            levelWon = true;
            SubmitScore();
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
        audioSrc.volume = ApplicationSettings.SoundVolume() * 0.4f;
        audioSrc.Play();
        GameObject freeze = Instantiate(freezeEffect, Vector3.zero, Quaternion.identity) as GameObject;
        Animator animator = freeze.GetComponent<Animator>();
        freeze.transform.parent = transform;
        for (float i = 0; i < 7; i += 0.01f) {
            foreach (Transform child in transform) {
                if (child.tag == "Ball") {
                    child.GetComponent<BallController>().Freeze();
                    if (i >= 5) {
                        child.transform.Find("Frost").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 * ((7 - i) / 2));
                        animator.SetBool("Unfreeze", true);
                    } else if (i < 0.3f) {
                        child.transform.Find("Frost").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 * (i / 0.3f));
                    }
                }
            }
            yield return new WaitForSeconds(0.01f);
        }

        foreach (Transform child in transform) {
            if (child.tag == "Ball") {
                child.GetComponent<BallController>().UnFreeze();
            }
        }
        Destroy(freeze);
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

    public void DestroyUnmovingVines() {
        foreach (Transform child in transform) {
            if (child.tag == "Vine" && !child.GetComponent<Grapple>().moving) {
                Destroy(child.gameObject);
            }
        }
    }

    public void ApplyBackground() {
        transform.Find("Backgrounds").GetChild(0).gameObject.SetActive(false);
        transform.Find("Backgrounds").GetChild(0).GetChild(0).gameObject.SetActive(false);

        transform.Find("Backgrounds").GetChild(theme-1).gameObject.SetActive(true);
        transform.Find("Backgrounds").GetChild(theme-1).GetChild(taintLevel-1).gameObject.SetActive(true);

        // ladders:
        foreach (Transform child in transform.Find("Ladders")) {
            // child.GetComponent<SpriteRenderer>().enabled = false;
            // child.GetChild(theme-1).GetComponent<SpriteRenderer>().enabled = true;
            // child.GetChild(theme-1).GetComponent<SpriteRenderer>().size = child.GetComponent<SpriteRenderer>().size;
        }
    }

    IEnumerator Transition() {
        GameObject transitionScreen = Instantiate(blackScreen, Vector3.zero, Quaternion.identity) as GameObject;

        Destroy(transitionScreen.transform.GetChild(0).gameObject);
        float opacity = 1f;
        while (true) {
            if (opacity <= 0) {
                yield return null;
            } else {
                opacity -= Time.unscaledDeltaTime;
                Color c = transitionScreen.GetComponent<SpriteRenderer>().color;
                transitionScreen.GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, opacity);
                
            }
            yield return null;
        }
    }
}

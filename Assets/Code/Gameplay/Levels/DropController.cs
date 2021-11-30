using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {
    private Animation spawnAnimation;
    bool collected = false;
    public int score;
    int spawnTime;
    bool destroying = false;

    void Start() {
        spawnTime = GameObject.Find("PlatformAndDropManager").GetComponent<DropManager>().time;
    }

    void Update() {
        if (GameObject.Find("PlatformAndDropManager").GetComponent<DropManager>().time - spawnTime > 25 && !destroying) {
            destroying = true;
            StartCoroutine(StartDestroyTimer());
        }
    }

    IEnumerator Spawn() {
        spawnAnimation.Play();
        yield return new WaitForSeconds(spawnAnimation.GetClip("AmmoSpawn").length);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.layer == 8 && !collected) {
            collected = true;
            transform.root.Find("Player").GetComponent<PlayerController>().HandleDrops(transform.gameObject);
            AddToScore();
            Destroy(transform.gameObject);
        }
    }

    void AddToScore() {
        if (score > 0) {
            transform.root.Find("UI/Canvas/PopupTextManager").GetComponent<PopupTextManager>().NewPopupText("+" + (score).ToString(), transform.position);
            GameObject.Find("PlayerUI").GetComponent<PlayerUI>().ChangeScore(score);
        }
    }

    IEnumerator StartDestroyTimer() {
        for (int i = 0; i < 5.5f; i++) {
            if (transform.tag == "ScoreItem") {
                GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
                transform.Find("lightbeam").gameObject.SetActive(false);
            } else {
                GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
            }
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(gameObject);
    }
 }

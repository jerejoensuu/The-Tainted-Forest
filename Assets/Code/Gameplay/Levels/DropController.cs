using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {
    private Animation spawnAnimation;
    bool collected = false;
    public int score;

    void Start() {
        // spawnAnimation = GetComponent<Animation>();

        // StartCoroutine(Spawn());
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropController : MonoBehaviour {
    private Animation spawnAnimation;
    private BoxCollider2D boxCollider2D;

    void Start() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
        spawnAnimation = GetComponent<Animation>();

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn() {
        spawnAnimation.Play();
        yield return new WaitForSeconds(spawnAnimation.GetClip("AmmoSpawn").length);
        boxCollider2D.enabled = true;
    }
}

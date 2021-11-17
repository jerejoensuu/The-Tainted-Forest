using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour {
    private Animation spawnAnimation;

    void Start() {
        spawnAnimation = GetComponent<Animation>();

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn() {
        spawnAnimation.Play();
        yield return new WaitForSeconds(spawnAnimation.GetClip("AmmoSpawn").length);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.layer == 8) {
            transform.root.Find("Player").GetComponent<PlayerController>().HandleDrops(transform.gameObject);
            Destroy(transform.gameObject);
        }
    }
}

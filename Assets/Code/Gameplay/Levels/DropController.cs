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
}

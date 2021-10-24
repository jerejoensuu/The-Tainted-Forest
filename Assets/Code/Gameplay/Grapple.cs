using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {
    public float speed = 8f;

    public AudioSource audioSrc;
    public AudioClip[] audioClips;
    void Awake() {
        PlaySound();
    }

    void PlaySound() {
        audioSrc.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSrc.Play();
    }

    void FixedUpdate() {

        Vector3 currentScale = new Vector3(0f, speed / 100, 0f);
        Vector3 currentPosition = new Vector3(0f, speed / 200, 0f);

        transform.localScale += currentScale;
        transform.localPosition += currentPosition;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Wall") {
            Destroy(transform.parent.gameObject);
        }

        if (col.gameObject.tag == "Ball") {
            Destroy(transform.parent.gameObject);
        }
    }
}

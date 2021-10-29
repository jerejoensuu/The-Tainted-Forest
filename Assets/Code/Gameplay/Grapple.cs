using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {
    public float speed = 8f;

    public AudioSource audioSrc;
    public AudioClip[] audioClips;
    private SpriteMask spriteMask;
    private BoxCollider2D boxCollider2D;
    float distanceMoved = 0;

    void Awake() {
        spriteMask = GetComponentInChildren<SpriteMask>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSrc = GetComponent<AudioSource>();
        PlaySound();
    }

    void PlaySound() {
        audioSrc.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSrc.Play();
    }

    void FixedUpdate() {
        Vector3 currentPosition = new Vector3(0f, speed / 200, 0f);
        Vector2 collisionOffset = new Vector3(0f, speed / 200, 0f);

        transform.localPosition += currentPosition;
        spriteMask.transform.position -= currentPosition;
        distanceMoved += currentPosition.y;

        boxCollider2D.offset = new Vector2(0, 0.5f * GetInverseProgress());
        boxCollider2D.size = new Vector2(1, 1f * GetProgress());
    }

    float GetProgress() {
        return distanceMoved / transform.localScale.y;
    }

    float GetInverseProgress() {
        return 1 - (distanceMoved / transform.localScale.y);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Wall") {
            Destroy(transform.gameObject);
        }

        if (col.gameObject.tag == "Ball") {
            Destroy(transform.gameObject);
        }
    }
}

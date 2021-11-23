using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireController : MonoBehaviour {
    public float speed = 20;
    public AudioSource audioSrc;
    public AudioClip[] audioClips;
    private BoxCollider2D boxCollider2D;
    public string side;
    float originalY;
    [Tooltip("Adjust starting height of spawned projectiles.")] public float projectileOffset;

    void Awake() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSrc = GetComponent<AudioSource>();
        PlaySound();
        speed /= 200;
        originalY = transform.localPosition.y;
    }

    void PlaySound() {
        audioSrc.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSrc.volume = ApplicationSettings.SoundVolume();
        audioSrc.Play();
    }

    void FixedUpdate() {
        speed *= 1.005f;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + speed, 17);
        if (transform.localPosition.y - originalY < 1f) {
            if (side == "left") {
                transform.localPosition = new Vector3(transform.localPosition.x - 0.025f, transform.localPosition.y, transform.localPosition.z);
            } else {
                transform.localPosition = new Vector3(transform.localPosition.x + 0.025f, transform.localPosition.y, transform.localPosition.z);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Wall" || col.gameObject.tag == "BreakableWall") {
            Destroy(transform.gameObject);
        }
    }
}

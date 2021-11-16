using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour {
    public float speed;
    public AudioSource audioSrc;
    public AudioClip[] audioClips;
    private SpriteMask spriteMask;
    private BoxCollider2D boxCollider2D;
    float distanceMoved = 0;
    public bool stickyVines = false;
    bool moving = true;
    [Tooltip("Adjust starting height of spawned projectiles.")] public float projectileOffset;

    void Awake() {
        spriteMask = GetComponentInChildren<SpriteMask>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSrc = GetComponent<AudioSource>();
        PlaySound();
    }

    void PlaySound() {
        audioSrc.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSrc.volume = ApplicationSettings.SoundVolume();
        audioSrc.Play();
    }

    void FixedUpdate() {
        if (moving) {
            speed *= 1.005f;
            Vector3 currentPosition = new Vector3(0f, speed / 200, 0f);
            Vector2 collisionOffset = new Vector3(0f, speed / 200, 0f);

            transform.localPosition += currentPosition;
            spriteMask.transform.position -= currentPosition;
            distanceMoved += currentPosition.y;

            boxCollider2D.size = new Vector2(0.3f, distanceMoved);
            boxCollider2D.offset = new Vector2(0, GetComponent<SpriteRenderer>().size.y/2 - distanceMoved/2 + projectileOffset);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "Wall") {
            if (stickyVines) {
                moving = false;
            } else {
                Destroy(transform.gameObject);
            }
            
        }
    }
}

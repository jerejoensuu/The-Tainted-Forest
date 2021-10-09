using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropController : MonoBehaviour {
    
    private ParticleSystem particle;
    private SpriteRenderer sr;
    private Animation animation;
    private BoxCollider2D boxCollider2D;

    void Start() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = false;
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;

        particle = GetComponentInChildren<ParticleSystem>();
        animation = GetComponent<Animation>();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn() {
        particle.Play();

        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);
        sr.enabled = true;
        animation.Play();
        boxCollider2D.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlockController : MonoBehaviour {

    private ParticleSystem particle;

    void Awake() {
        particle = GetComponentInChildren<ParticleSystem>();   
    }

    void OnTriggerEnter2D(Collider2D col) {
        // Collision with player projectiles
        if (col.gameObject.tag == "Projectile") {
            StartCoroutine(Break());
        }
    }

    IEnumerator Break() {
        particle.Play();

        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}

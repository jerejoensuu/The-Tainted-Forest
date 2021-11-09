using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlockController : MonoBehaviour {

    [SerializeField] public bool allowDrops;
    private ParticleSystem particle;
    private SpriteRenderer sr;

    void Awake() {
        particle = GetComponentInChildren<ParticleSystem>();
        sr = GetComponentInChildren<SpriteRenderer>();

        var sh = particle.shape;
        sh.scale = new Vector3 (transform.localScale.x, transform.localScale.y, 0);
    }

    void OnTriggerEnter2D(Collider2D col) {
        // Collision with player projectiles
        if (col.gameObject.tag == "Projectile") {
            StartCoroutine(Break());
        }
    }

    IEnumerator Break() {
        particle.Play();
        sr.enabled = false;

        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}

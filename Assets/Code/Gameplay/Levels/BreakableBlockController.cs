using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlockController : MonoBehaviour {

    [SerializeField] public bool allowDrops;
    private ParticleSystem particle;
    private SpriteRenderer sr;
    [SerializeField] Material mat1;
    [SerializeField] Material mat2;

    void Awake() {
        GetComponent<AudioSource>().volume = ApplicationSettings.MusicVolume() * 0.3f;
        try {
            bool test = transform.parent.name != "PlatformAndDropManager";
        }
        catch (System.Exception) {
            Debug.Log("OBJECT NOT SET AS CHILD OF PlatformAndDropManager!");
            throw;
        }

        particle = GetComponentInChildren<ParticleSystem>();
        sr = GetComponentInChildren<SpriteRenderer>();

        var sh = particle.shape;
        sh.scale = new Vector3 (GetComponent<SpriteRenderer>().size.x/2, GetComponent<SpriteRenderer>().size.y/2, 0);
        
        switch (transform.parent.parent.GetComponent<LevelManager>().theme) {
            case 1: particle.GetComponent<ParticleSystemRenderer>().material = mat1;
                    break;
            case 2: particle.GetComponent<ParticleSystemRenderer>().material = mat2;
                    break;
        }
        
    }

    void OnTriggerEnter2D(Collider2D col) {
        // Collision with player projectiles
        if (col.gameObject.tag == "Vine") {
            StartCoroutine(Break());
        }
    }

    IEnumerator Break() {
        particle.Play();
        GetComponent<AudioSource>().Play();
        foreach (Transform child in transform) {
            if (child.tag == "Theme") {
                child.GetComponent<SpriteRenderer>().enabled = false;
            }
        }

        yield return new WaitForSeconds(particle.main.startLifetime.constantMax);
        Destroy(gameObject);
    }
}

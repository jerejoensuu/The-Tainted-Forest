using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestroyAudio : MonoBehaviour {

    // Sound is played from a separate object because destroyed objects can't play sounds

    public AudioSource audioSrc;
    public AudioClip[] audioClips;

    public void PlaySound() {
        audioSrc.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSrc.Play();
        transform.SetParent(null);
        Destroy(gameObject, 0.5f);
    }
}

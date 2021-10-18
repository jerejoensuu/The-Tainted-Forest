using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public int time { get; private set; }

    void Start() {
        StartCoroutine(TrackTime());
    }

    IEnumerator TrackTime() {

        while (true) {
            yield return new WaitForSeconds(1);
            time++;
        }

    }
}

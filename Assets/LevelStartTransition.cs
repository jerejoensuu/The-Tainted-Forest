using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelStartTransition : MonoBehaviour {

    TextMeshProUGUI txt;
    public GameObject textObject;
    Animation animation;
    
    void Start() {
        animation = transform.GetChild(0).GetComponent<Animation>();
        Time.timeScale = 0;
        StartLevel();
    }

    
    void Update() {
        //textObject.transform.position = new Vector3(textObject.transform.position.x + 1, textObject.transform.position.y, 0);
    }

    void StartLevel() {
        //animation.Play();
    }
}

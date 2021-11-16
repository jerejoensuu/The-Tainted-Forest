using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour {
    
    [SerializeField] public bool allowDrops;

    void Awake() {
        if (transform.parent.name != "PlatformAndDropManager") {
            Debug.Log("OBJECT NOT SET AS CHILD OF PlatformAndDropManager!");
        }
    }

}

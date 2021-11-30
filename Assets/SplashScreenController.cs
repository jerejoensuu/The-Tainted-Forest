using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreenController : MonoBehaviour {
    
    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ActivateCursor() {
        Cursor.lockState = CursorLockMode.None;
    }

}

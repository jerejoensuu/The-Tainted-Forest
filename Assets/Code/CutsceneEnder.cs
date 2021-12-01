using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEnder : MonoBehaviour {
    public void AllowCutsceneSkip() {
        GameObject.Find("Canvas").GetComponent<CutsceneController>().AllowCutsceneSkip();
    }
}

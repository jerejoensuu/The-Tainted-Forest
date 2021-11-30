using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour {

    public GameObject blackScreen;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (Input.anyKeyDown) {
            StartCoroutine(LoadLevelOne());
        }
    }

    IEnumerator LoadLevelOne() {
        GameObject transitionScreen = Instantiate(blackScreen, Vector3.zero, Quaternion.identity) as GameObject;

        float maskSize = 1f;
        while (true) {
            maskSize -= Time.unscaledDeltaTime;
            if (maskSize <= 0) {
                Destroy(transform.transform.gameObject);
                break;
            } else {
                transitionScreen.GetComponentInChildren<SpriteMask>().transform.localScale = new Vector2(maskSize,maskSize);
            }
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("1");
        asyncLoad.allowSceneActivation = true;
        while(false) {
            // do something
            //asyncLoad.allowSceneActivation = true;
        }
    }
}

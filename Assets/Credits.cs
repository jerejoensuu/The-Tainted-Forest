using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {

    void Awake() {
        Time.timeScale = 1;
    }
    
    public void EndCredits() {
        SceneManager.LoadScene("MainMenu");
    }

}

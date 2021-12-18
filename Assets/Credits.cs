using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour {
    
    public void EndCredits() {
        SceneManager.LoadScene("MainMenu");
    }

}

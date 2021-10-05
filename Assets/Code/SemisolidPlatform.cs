using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemisolidPlatform : MonoBehaviour {
    public GameObject player; // Todo: For multiplayer, make a static list & make each player add themselves to it when spawning. Check collision separately for each player.

    void Update() {
        if (player.transform.position.y < transform.position.y + transform.localScale.y / 2 || player.GetComponent<PlayerController>().climbing) {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>(), true);
        }
        else {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>(), false);
        }
    }
}

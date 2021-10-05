using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemisolidPlatform : MonoBehaviour {

    public static List<GameObject> playerObjects = new List<GameObject>();

    void Awake () {
        SemisolidPlatform.playerObjects.Clear();
        SemisolidPlatform.playerObjects.TrimExcess();
    }
    void Update() {
        foreach (GameObject player in playerObjects) {
            if (player.transform.position.y < transform.position.y + transform.localScale.y / 2 || player.GetComponent<PlayerController>().climbing) {
                Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>(), true);
            }
            else {
                Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<BoxCollider2D>(), false);
            }
        }
    }
}

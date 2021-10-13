using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropSpawnerController : MonoBehaviour {

    public static List<GameObject> spawnerList = new List<GameObject>();
    
    void Start() {
        
        //Debug.Log(spawnerList[1]);
        //Instantiate(ammoDrop, spawnerList[Random.Range(0, spawnerList.Count)].transform.position, Quaternion.identity);
    }

    void Update() {
        
    }

    static void AddToList(GameObject go) {
        spawnerList.Add(go);
    }
}

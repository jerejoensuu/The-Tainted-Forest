using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropSpawner : MonoBehaviour {

    public GameObject spawner;
    [SerializeField] private GameObject ammoDrop;
    static List<GameObject> spawnerList = new List<GameObject>();
    int listSize;
    bool spawned = false;

    void Start() {
        spawner = gameObject;
        spawnerList.Add(spawner);
        listSize = spawnerList.Count;
    }

    void Update() {
        
        if (!spawned) {
            spawned = true;
            Debug.Log(spawnerList[1]);
            Instantiate(ammoDrop, spawnerList[Random.Range(0, listSize)].transform.position, Quaternion.identity);
        }

    }
}

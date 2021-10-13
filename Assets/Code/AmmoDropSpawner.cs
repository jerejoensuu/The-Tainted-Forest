using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropSpawner : MonoBehaviour {

    public GameObject spawner;
    [SerializeField] private GameObject ammoDrop;
    int listSize;
    bool spawned = false;

    void Awake() {
        try {
            transform.SetParent(GetComponentInParent<Transform>());
            Debug.Log(GetComponentInParent<Transform>());
        } catch (System.Exception e) {
            throw new System.Exception("AmmoDropSpawner not set as child of AmmoDropSpawnerParent.", e);
        }

        spawner = gameObject;
        //GetComponentInParent<AmmoDropSpawnerController>()
    }

    void Update() {
        

    }
}

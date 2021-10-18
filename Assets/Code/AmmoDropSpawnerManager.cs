using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDropSpawnerManager : MonoBehaviour {

    private List<GameObject> Spawners = new List<GameObject>();
    [SerializeField] private GameObject ammoDrop;
    public int time { get; private set;}
    [Tooltip("How may seconds should the script wait before attempting to spawn ammo drops again after spawning one.")]
    [SerializeField] private int cooldown;
    private int currentCooldown = 0;
    [Tooltip("Change of ammo drops spawning per second as percentage.")]
    [SerializeField] private float changeOfSpawn;

    
    void Start() {
        
        foreach (Transform child in transform) {
            Spawners.Add(child.gameObject);
        }

        StartCoroutine(TrackTime());

    }

    IEnumerator TrackTime() {

        while (true) {
            yield return new WaitForSeconds(1);
            time++;

            if (currentCooldown > 0) {
                currentCooldown--;
            } else {
                AttemptSpawn();
            }
        }

    }

    void AttemptSpawn() {
        if (Random.Range(0, (int)(1 / changeOfSpawn)) == 0) {
            Instantiate(ammoDrop, Spawners[Random.Range(0,Spawners.Count)].transform.position, Quaternion.identity);
            currentCooldown = cooldown;
        }
    }
}

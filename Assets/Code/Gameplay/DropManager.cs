using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour {

    private List<GameObject> Spawns = new List<GameObject>();
    [SerializeField] private GameObject ammoDrop;
    public int time { get; private set;}
    [Tooltip("How may seconds should the script wait before attempting to spawn ammo drops again after spawning one.")]
    [SerializeField] private int cooldown;
    private int currentCooldown = 0;
    [Tooltip("Change of ammo drops spawning per second as percentage.")]
    [SerializeField] private float changeOfSpawn;
    private ParticleSystem particleRays;
    private ParticleSystem particleCircle;
    
    void Start() {
        particleRays = transform.Find("DropAnimation").Find("Particle System").GetComponent<ParticleSystem>();
        particleCircle = transform.Find("DropAnimation").Find("Center sphere").GetComponent<ParticleSystem>();
        
        foreach (Transform child in transform) {
            if (child.gameObject.GetComponent<PlatformController>() != null) {
                if (child.gameObject.GetComponent<PlatformController>().allowDrops == true) {
                    Spawns.Add(child.gameObject);
                }
            }
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
            StartCoroutine(Spawn());
            currentCooldown = cooldown;
        }
    }

    IEnumerator Spawn() {
        GameObject spawnPlatform = Spawns[Random.Range(0,Spawns.Count)];
        Vector2 location = new Vector2(Random.Range(spawnPlatform.transform.position.x - spawnPlatform.transform.localScale.x/2 + ammoDrop.transform.localScale.x/2,
                                                    spawnPlatform.transform.position.x + spawnPlatform.transform.localScale.x/2)
                                       ,spawnPlatform.transform.position.y + spawnPlatform.transform.localScale.y/2  + ammoDrop.transform.localScale.y/2);

        particleRays.transform.position = location;
        particleCircle.transform.position = location;
        particleRays.Play();
        particleCircle.Play();

        yield return new WaitForSeconds(particleCircle.main.startLifetime.constantMax);
        
        Instantiate(ammoDrop, location, Quaternion.identity);

    }
}

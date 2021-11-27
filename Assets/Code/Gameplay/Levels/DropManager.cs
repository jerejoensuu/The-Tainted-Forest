using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour {

    private List<GameObject> Spawns = new List<GameObject>();

    private GameObject ammoDrop;

    public List<GameObject> dropPrefabs;
    public List<int> dropWeights;
    public List<int> dropScores;
    public List<GameObject> scoreItems;
    public List<int> scoreItemPoints;

    public int time { get; private set;}
    [Tooltip("How may seconds should the script wait before attempting to spawn ammo drops again after spawning one.")] [SerializeField] private int cooldown;
    private int currentCooldown = 0;
    [Tooltip("Change of a drop spawning per second as percentage.")] [SerializeField] private float changeOfSpawn;
    private ParticleSystem particleRays;
    private ParticleSystem particleCircle;
    bool lowAmmo = false;

    private List<Drop> dropPool = new List<Drop>();
    public class Drop {
        public GameObject drop { get; set; }
        //public int weight { get; set; }
        public int score { get; set; }
    }

    // TEMP
    Vector2 tempLocation = Vector2.zero;
    
    void Start() {
        ammoDrop = dropPrefabs[0];

        for (int i = 0; i < dropPrefabs.Count; i++) {
            for (int j = 0; j < dropWeights[i]; j++) {
                dropPool.Add(new Drop{drop = dropPrefabs[i], score = dropScores[i]});
            }
        }

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
        lowAmmo = false;

        while (true) {
            yield return new WaitForSeconds(1);
            time++;
            if (transform.root.Find("Player").GetComponent<PlayerController>().ammoCount < 3 && !transform.root.GetComponent<LevelManager>().FindDrops(ammoDrop) && !lowAmmo) {
                lowAmmo = true;
                currentCooldown = 0 + Random.Range(3, 6);
            }
            Debug.Log(currentCooldown);
            if (currentCooldown > 0) {
                currentCooldown--;
            } else {
                AttemptSpawn();
            }
        }

    }

    void AttemptSpawn() {
        if (Random.Range(0, (int)(1 / changeOfSpawn)) == 0 || lowAmmo) {
            StartCoroutine(Spawn());
            currentCooldown = cooldown + (int)particleCircle.main.startLifetime.constantMax;
        }
    }

    IEnumerator Spawn() {
        GameObject dropObject = GetRandomDrop();
        Vector2 location = GetRandomSpawnLocation(dropObject);
        
        particleRays.transform.position = location;
        particleCircle.transform.position = location;
        particleRays.Play();
        particleCircle.Play();

        yield return new WaitForSeconds(particleCircle.main.startLifetime.constantMax);
      
        GameObject drop = Instantiate(dropObject, location, Quaternion.identity) as GameObject;
        drop.transform.parent = transform.root.transform;
        lowAmmo = false;

    }

    Vector2 GetRandomSpawnLocation(GameObject dropObject) {
        Vector2 location = Vector2.zero;
        GameObject spawnPlatform = Spawns[Random.Range(0,Spawns.Count)];
        int failsafe = 1000000;

        while(true) {
            if (spawnPlatform.layer == 3) {
                location = new Vector2(Random.Range(spawnPlatform.transform.position.x - spawnPlatform.GetComponent<SpriteRenderer>().size.x/2 + dropObject.transform.localScale.x/2,
                                                    spawnPlatform.transform.position.x + spawnPlatform.GetComponent<SpriteRenderer>().size.x/2 - dropObject.transform.localScale.x/2)
                                       ,spawnPlatform.transform.position.y + spawnPlatform.transform.localScale.y/2  + dropObject.transform.localScale.y/2*1.1f);
            
            } else {
                location = new Vector2(Random.Range(spawnPlatform.transform.position.x - spawnPlatform.transform.localScale.x/2 + dropObject.transform.localScale.x/2,
                                                        spawnPlatform.transform.position.x + spawnPlatform.transform.localScale.x/2 - dropObject.transform.localScale.x/2)
                                        ,spawnPlatform.transform.position.y + spawnPlatform.transform.localScale.y/2  + dropObject.transform.localScale.y/2*1.1f);
            }

            if (!Physics2D.BoxCast(location, new Vector3(1, 1, 1), 0, Vector2.zero, 0, 3) || failsafe == 0) {
                tempLocation = location;
                break;
            }
            failsafe--;
        }
        

        return location;
    }

    void OnDrawGizmos() {
        Gizmos.DrawWireCube(tempLocation, new Vector3(1, 1, 1));
    }

    public GameObject GetRandomDrop() {
        GameObject drop;
        if (lowAmmo) {
            drop = ammoDrop;
        } else {
            int failsafe = 100;

            if (Random.Range(0f, 1f) < 0.3f) { // 30% chance of spawning a score item
                int index = Random.Range(0, scoreItems.Count);
                drop = scoreItems[index];
                drop.GetComponent<DropController>().score = scoreItemPoints[index];
            } else {
                while(true) {
                    int index = Random.Range(0, dropPool.Count);
                    drop = dropPool[index].drop;
                    drop.GetComponent<DropController>().score = dropPool[index].score;
                    if (!transform.root.GetComponent<LevelManager>().FindDrops(drop) || failsafe == 0) {
                        break;
                    }
                    failsafe--;
                }
            }
            
        }
        return drop;  
    }

    public void ApplyTheme(int theme) {
        foreach (Transform child in transform) {
            if (child.gameObject.layer == 3 && child.GetComponentInChildren<SpriteRenderer>() != null) {
                child.GetComponent<SpriteRenderer>().enabled = false;
                child.GetChild(theme-1).GetComponent<SpriteRenderer>().enabled = true;
                child.GetChild(theme-1).GetComponent<SpriteRenderer>().size = child.GetComponent<SpriteRenderer>().size;
            }
        }
    }
}

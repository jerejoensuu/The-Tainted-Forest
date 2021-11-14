using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropManager : MonoBehaviour {

    private List<GameObject> Spawns = new List<GameObject>();

    [SerializeField] private GameObject ammoDrop;
    [SerializeField] private int ammoDropWeight;
    [SerializeField] private GameObject damageAll;
    [SerializeField] private int damageAllWeight;
    [SerializeField] private GameObject timeFreeze;
    [SerializeField] private int timeFreezeWeight;
    [SerializeField] private GameObject shield;
    [SerializeField] private int shieldWeight;

    public int time { get; private set;}
    [Tooltip("How may seconds should the script wait before attempting to spawn ammo drops again after spawning one.")] [SerializeField] private int cooldown;
    private int currentCooldown = 0;
    [Tooltip("Change of a drop spawning per second as percentage.")] [SerializeField] private float changeOfSpawn;
    private ParticleSystem particleRays;
    private ParticleSystem particleCircle;

    private List<Drop> drops = new List<Drop>();
    private List<GameObject> dropPool = new List<GameObject>();
    public class Drop {
        public GameObject drop { get; set; }
        public int weight { get; set; }
    }
    
    void Start() {
        drops.Add(new Drop{drop = ammoDrop, weight = ammoDropWeight});
        drops.Add(new Drop{drop = damageAll, weight = damageAllWeight});
        drops.Add(new Drop{drop = timeFreeze, weight = timeFreezeWeight});
        drops.Add(new Drop{drop = shield, weight = shieldWeight});

        foreach (Drop drop in drops) {
            for (int i = 0; i < drop.weight; i++) {
                dropPool.Add(drop.drop);
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
            currentCooldown = cooldown + (int)particleCircle.main.startLifetime.constantMax;
        }
    }

    IEnumerator Spawn() {
        Vector2 location;
        GameObject spawnPlatform = Spawns[Random.Range(0,Spawns.Count)];
        GameObject drop;        
        
        if (spawnPlatform.layer == 3) {
            location = new Vector2(Random.Range(spawnPlatform.transform.position.x - spawnPlatform.GetComponent<SpriteRenderer>().size.x/2 + ammoDrop.transform.localScale.x/2,
                                                    spawnPlatform.transform.position.x + spawnPlatform.GetComponent<SpriteRenderer>().size.x/2 - ammoDrop.transform.localScale.x/2)
                                       ,spawnPlatform.transform.position.y + spawnPlatform.transform.localScale.y/2  + ammoDrop.transform.localScale.y/2);
        } else {
            location = new Vector2(Random.Range(spawnPlatform.transform.position.x - spawnPlatform.transform.localScale.x/2 + ammoDrop.transform.localScale.x/2,
                                                    spawnPlatform.transform.position.x + spawnPlatform.transform.localScale.x/2 - ammoDrop.transform.localScale.x/2)
                                       ,spawnPlatform.transform.position.y + spawnPlatform.transform.localScale.y/2  + ammoDrop.transform.localScale.y/2);
        }
        
        particleRays.transform.position = location;
        particleCircle.transform.position = location;
        particleRays.Play();
        particleCircle.Play();

        yield return new WaitForSeconds(particleCircle.main.startLifetime.constantMax);

        if (transform.parent.transform.Find("Player").GetComponent<PlayerController>().ammoCount == 0) {
            drop = ammoDrop;
        } else {
            drop = dropPool[Random.Range(0, dropPool.Count)];
        }        
        Instantiate(drop, location, Quaternion.identity);

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

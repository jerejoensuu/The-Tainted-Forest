using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireManager : MonoBehaviour {
    public GameObject bulletPrefab;
    [Tooltip("Adjust starting height of spawned projectiles.")] public float projectileOffset;

    void Awake() {
        
    }

    public void Fire() {
        GameObject bullet1;
        bullet1 = Instantiate(bulletPrefab,
                            new Vector3(transform.position.x, transform.position.y + projectileOffset, 0f),
                            Quaternion.identity) as GameObject;
        bullet1.transform.parent = transform.parent;
        bullet1.GetComponent<RapidFireController>().side = "left";

        GameObject bullet2;
        bullet2 = Instantiate(bulletPrefab,
                            new Vector3(transform.position.x, transform.position.y + projectileOffset, 0f),
                            Quaternion.identity) as GameObject;
        bullet2.transform.parent = transform.parent;
        bullet2.GetComponent<RapidFireController>().side = "right";
    }

    public IEnumerator Activate() {
        GetComponent<PlayerController>().projectileType = "RapidFire";
        yield return new WaitForSeconds(5f);
        GetComponent<PlayerController>().projectileType = "Vine";
    }
}

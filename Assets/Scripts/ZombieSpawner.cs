using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject zombie;
    public float spawnRadius = 5f;
    private int counterOfZombies;
    public int maxAmountOfZombies = 2;
    void Start()
    {
        //SpawnZombie();
    }
    
    public void SpawnZombie()
    {
        Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
        randomOffset.y = 0;
        Instantiate(zombie, transform.position + randomOffset, Quaternion.identity);
        counterOfZombies += 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(counterOfZombies < maxAmountOfZombies){
            SpawnZombie();
        }
    }

    public void KillZombie(){
        counterOfZombies -= 1;
        Debug.Log("Amount of Zombies " + counterOfZombies);
    }
}

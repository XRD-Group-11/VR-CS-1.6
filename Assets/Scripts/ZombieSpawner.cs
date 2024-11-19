using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject zombie;
    public float spawnRadius = 5f;
    private int counterOfZombies;
    private int totalCountOfKilledZombies;
    public int maxAmountOfZombies = 3;
    void Start()
    {
        //SpawnZombie();
        totalCountOfKilledZombies = 0;
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
        totalCountOfKilledZombies += 1;
        if(totalCountOfKilledZombies==1){
            AudioManager.instance.Play("firstBlood");
        }
        Dictionary<int, string> sounds = new Dictionary<int, string>()
        {
            {1,"HolyShit"},
            {2,"GodLike"},
            {3,"KillingSpree"},
            {4,"Dominating"},
            {5,"Unstoppable"}
        };
        if(totalCountOfKilledZombies%4==0 ){
            int randomInRange = Random.Range(1, 5); 
            AudioManager.instance.Play(sounds[randomInRange]);
        }
        Debug.Log("Amount of Zombies " + counterOfZombies);
    }
}

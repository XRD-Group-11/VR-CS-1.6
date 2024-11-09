using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;

    private bool isExploded = false;
    private float timer = 2f;

    public void Explode()
    {
        isExploded = true;
    }
    
    private void Update()
    {
        if (isExploded)
        {
            if (timer <= 0)
            {
                AudioManager.instance.Play("Grenade_Exploding");
                _particleSystem.Play();
                isExploded = false;
                Destroy(this.gameObject, _particleSystem.main.duration);
            }

            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
            
            
            
            
        }
    }
}
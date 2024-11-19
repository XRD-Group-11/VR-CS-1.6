using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private int fireRate = 600; // 600 rounds per minute (RPM) 
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject hitParticalEffect;
    [SerializeField] private GameObject zombieHitParticalEffect;
    [SerializeField] private float recoil = 1f;
    [SerializeField] private float recoilSpeed = 0.1f;
    public int bulletDamage = 20;
    
    private Rigidbody _rigidbody;
    private float timer = 0f;
    private float recoilScale = 0f;

    private bool isShooting = false;

    public void StartShoot()
    {
        isShooting = true;
    }
    
    public void EndShoot()
    {
        isShooting = false;
    }

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isShooting)
        {
            if (timer <= 0)
            {
                AudioManager.instance.Play("ak47_shoot");
                _particleSystem.Play();
                
                // Add recoil to the raycast's angle
                Vector3 recoilOffset = transform.forward + new Vector3(
                    Random.Range(-recoilScale * (recoil / 20), recoilScale * (recoil / 20)), // Horizontal recoil
                    Random.Range(-recoilScale * (recoil / 10), recoilScale * (recoil / 10)), // Vertical recoil
                    0); // No change in depth
                
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position,recoilOffset, out hit))
                {
                    if(!hit.collider.CompareTag("Zombie"))
                        Instantiate(hitParticalEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
                if(hit.collider.CompareTag("Zombie")) {
                    GameObject hitObject = hit.collider.gameObject;
                    // Get the script component and call the function
                    ZombieNPCScript script = hitObject.GetComponent<ZombieNPCScript>();
                    if (script != null)
                    {
                        script.TakeDamage(bulletDamage);
                        Instantiate(zombieHitParticalEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));

                    }
                    else
                    {
                        Debug.LogWarning("YourScript not found on the hit object!");
                    }
                }
    {
        Debug.Log("Hit an enemy!");
        // Add logic for when the object hit is an enemy
    }
                
                timer = 60f / fireRate;
                recoilScale += recoilSpeed * Time.deltaTime;
                recoilScale = recoilScale >= 1 ? 1 : recoilScale;
            }
        }

        if (timer >= 0)
            timer -= Time.deltaTime;

        if (recoilScale > 0 && !isShooting)
            recoilScale -= Time.deltaTime;
        else if(recoilScale < 0)
            recoilScale = 0;
    }
}

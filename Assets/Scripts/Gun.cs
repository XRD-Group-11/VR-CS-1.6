using System;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private int fireRate = 600; // 600 rounds per minute (RPM) 
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject hitParticalEffect;
    
    private Rigidbody _rigidbody;
    private float timer = 0f;

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
                
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, firePoint.forward, out hit))
                {
                    Instantiate(hitParticalEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
                
                timer = 60f / fireRate;
            }
        }

        if (timer >= 0)
            timer -= Time.deltaTime;
    }
}

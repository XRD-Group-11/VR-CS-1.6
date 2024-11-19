using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float force = 500f;
    private bool isExploded = false;
    private float timer = 2f;
    IDamageable takeDamage;

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

                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                foreach (Collider collider in colliders)
                {
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.AddExplosionForce(force, transform.position, radius);
                    
                    IDamageable damageable = collider.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(100); 
                    }
                }

                Destroy(this.gameObject, _particleSystem.main.duration);
            }

            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
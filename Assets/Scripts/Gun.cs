using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Gun : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private int fireRate = 600; // 600 rounds per minute (RPM) 
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private GameObject hitParticalEffect;
    [SerializeField] private float recoil = 1f;
    [SerializeField] private float recoilSpeed = 0.1f;
    
    private Rigidbody _rigidbody;
    private XRSocketInteractor _xrSocketInteractor;
    private float timer = 0f;
    private float recoilScale = 0f;
    private Magazine _magazine;
    private bool hasBullets = false;
    private bool bulletInChamber = true;

    private bool isShooting = false;

    public void StartShoot()
    {
        isShooting = true;
        if (!hasBullets && !bulletInChamber)
        {
        AudioManager.instance.Play("pinpull");
        }
    }

    public void BoltPull()
    {
        if (hasBullets && !bulletInChamber)
            bulletInChamber = true;
    }
    
    public void EndShoot()
    {
        isShooting = false;
    }
    
    public void MagOut() { 
        AudioManager.instance.Play("ak47_magout");
        _magazine = null;
        hasBullets = false;
    }
    
    public void MagIn() {
        AudioManager.instance.Play("ak47_magin");
        _magazine = _xrSocketInteractor.interactablesSelected[0].transform.GetComponent<Magazine>();
        if(_magazine.ammo > 0)
            hasBullets = true;
        else
            hasBullets = false;
    }

    private void Awake()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _xrSocketInteractor = gameObject.GetComponent<XRSocketInteractor>();
    }

    private void Update()
    {
        if (isShooting)
        {
            if (!hasBullets && !bulletInChamber)
                return;
            
            if (timer <= 0)
            {
                if(!bulletInChamber)
                    return;
                
                if (!hasBullets)
                {
                    bulletInChamber = false;
                }
                
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
                    Instantiate(hitParticalEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }
                
                timer = 60f / fireRate;
                recoilScale += recoilSpeed * Time.deltaTime;
                recoilScale = recoilScale >= 1 ? 1 : recoilScale;
                
                if (_magazine.ammo <= 0)
                    hasBullets = false;
                else 
                    _magazine.TakeOneBullet();
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
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
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

    [SerializeField] private Sprite[] digitSprites;
    [SerializeField] private Image[] digitImages;
    [SerializeField] private Image[] digitImagesAmmoLeft;

    [SerializeField] private GameObject _ammo;
    //[SerializeField] private XRGrabInteractable grabMagazine;

    private Rigidbody _rigidbody;
    private XRSocketInteractor _xrSocketInteractor;
    private float timer = 0f;
    private float reloadTimer = 0f;
    private float recoilScale = 0f;

    private Magazine _magazine;
    private bool hasBullets = false;
    private bool bulletInChamber = true;

    private bool isShooting = false;
    private int ammoSpent = 0;
    private bool magazineInWeapon = true;

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

        UpdateNumericDisplay(0, 30);
    }
    
    public void MagIn() {
        AudioManager.instance.Play("ak47_magin");
        _magazine = _xrSocketInteractor.interactablesSelected[0].transform.GetComponent<Magazine>();
        if(_magazine.ammo > 0)
            hasBullets = true;
        else
            hasBullets = false;

        UpdateNumericDisplay(_magazine.ammo, 30);
    }

    private void Awake()
    {
        //_ammo.SetActive(false);
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _xrSocketInteractor = gameObject.GetComponent<XRSocketInteractor>();
    }

    private void Update()
    {
        if(isShooting) {
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
                if (Physics.Raycast(firePoint.position, recoilOffset, out hit))
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

                timer = 60f / fireRate;
                recoilScale += recoilSpeed * Time.deltaTime;
                recoilScale = recoilScale >= 1 ? 1 : recoilScale;
                
                if (hasBullets && _magazine.ammo <= 0)
                    hasBullets = false;
                else 
                    if(hasBullets) _magazine.TakeOneBullet();

                    UpdateNumericDisplay(_magazine.ammo, 30);
            }
        }


        if (timer >= 0)
            timer -= Time.deltaTime;

        if (recoilScale > 0 && !isShooting)
            recoilScale -= Time.deltaTime;
        else if (recoilScale < 0)
            recoilScale = 0;
    }

    private void UpdateNumericDisplay(int bullets, int bulletsRemaining)
    {
        var tens = (bullets % 100) / 10;
        var ones = bullets % 10;

        var tens2 = (bulletsRemaining % 100) / 10;
        var ones2 = bulletsRemaining % 10;


        if (bulletsRemaining >= 10)
        {
            digitImagesAmmoLeft[0].gameObject.SetActive(true);
            digitImagesAmmoLeft[0].sprite = digitSprites[tens2];
        }
        else
        {
            digitImagesAmmoLeft[0].gameObject.SetActive(false);
        }

        // Update the tens digit if bullets >= 10
        if (bullets >= 10)
        {
            digitImages[0].gameObject.SetActive(true);
            digitImages[0].sprite = digitSprites[tens];
        }
        else
        {
            digitImages[0].gameObject.SetActive(false);
        }

        // Always show ones place
        digitImages[1].gameObject.SetActive(true);
        digitImagesAmmoLeft[1].gameObject.SetActive(true);
        digitImages[1].sprite = digitSprites[ones];
        digitImagesAmmoLeft[1].sprite = digitSprites[ones2];
    }
}
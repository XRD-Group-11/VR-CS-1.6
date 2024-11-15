using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private float recoil = 1f;
    [SerializeField] private float recoilSpeed = 0.1f;

    [SerializeField] private int currentAmmo = 20;
    [SerializeField] private int leftAmmo = 51;

    [SerializeField] private Sprite[] digitSprites;
    [SerializeField] private Image[] digitImages;
    [SerializeField] private Image[] digitImagesAmmoLeft;

    [SerializeField] private GameObject _ammo;
    [SerializeField] private XRGrabInteractable grabMagazine;

    private bool isReloading;
    private bool isPickedUp = false;
    private Rigidbody _rigidbody;
    private float timer = 0f;
    private float reloadTimer = 0f;
    private float recoilScale = 0f;
    private XRGrabInteractable grabInteractable;
    private bool isShooting = false;
    private int ammoSpent = 0;
    private bool magazineInWeapon = true;

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
        _ammo.SetActive(false);
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectExited.AddListener(OnGunDropped);
        grabInteractable.selectEntered.AddListener(OnGunPickedUp);
        
        grabMagazine.selectExited.AddListener(OnMagazineRemoved);
        grabMagazine.selectEntered.AddListener(OnMagazineInserted);

        UpdateNumericDisplay(currentAmmo, leftAmmo);
    }

    private void OnGunPickedUp(SelectEnterEventArgs args)
    {
        isPickedUp = true;
        _ammo.SetActive(true);
    }

    private void OnGunDropped(SelectExitEventArgs args)
    {
        isPickedUp = false;
        _ammo.SetActive(false);
    }
    private void OnMagazineRemoved(SelectExitEventArgs args)
    {
        magazineInWeapon = false;
        Debug.Log("Magazine removed!");
    }

    private void OnMagazineInserted(SelectEnterEventArgs args)
    {
        magazineInWeapon = true;
        Debug.Log("Magazine inserted!");
        if (CanReload(currentAmmo))
        {
            StartReload();
        }
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGunPickedUp);
        grabInteractable.selectExited.RemoveListener(OnGunDropped);
        grabMagazine.selectExited.RemoveListener(OnMagazineRemoved);
        grabMagazine.selectEntered.RemoveListener(OnMagazineInserted);
    }

    private void Update()
    {
        if (isShooting && currentAmmo > 0 && isReloading == false)
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
                if (Physics.Raycast(firePoint.position, recoilOffset, out hit))
                {
                    Instantiate(hitParticalEffect, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                }

                currentAmmo--;
                UpdateNumericDisplay(currentAmmo, leftAmmo);
                if (currentAmmo < 0) currentAmmo = 0;     
                

                timer = 60f / fireRate;
                recoilScale += recoilSpeed * Time.deltaTime;
                recoilScale = recoilScale >= 1 ? 1 : recoilScale;
            }
        }

        if (leftAmmo <= 0) leftAmmo = 0;

        if (Input.GetKeyDown(KeyCode.O) && isPickedUp && !isReloading && CanReload(currentAmmo))
        {
            Debug.Log("Reload Start");
            StartReload();
        }

        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                FinishReload();
            }
        }

        if (timer >= 0)
            timer -= Time.deltaTime;

        if (recoilScale > 0 && !isShooting)
            recoilScale -= Time.deltaTime;
        else if (recoilScale < 0)
            recoilScale = 0;
    }

    private void StartReload()
    {
        if (!magazineInWeapon) return; 

        isReloading = true;
        reloadTimer = 0f; 
        Debug.Log("Reloading...");
    }

    private void FinishReload()
    {
        isReloading = false;
        UpdateLeftAmmo(currentAmmo, leftAmmo);

        Debug.Log("Finished Reloading");
    }

    private void UpdateLeftAmmo(int bullets, int ammoLeft)
    {
        int ammoNeeded = 30 - bullets; 
        ammoSpent = Mathf.Min(ammoNeeded, ammoLeft); 
        
        currentAmmo += ammoSpent;
        leftAmmo -= ammoSpent;

        UpdateNumericDisplay(currentAmmo, leftAmmo);
    }

    private bool CanReload(int bulletCount)
    {
        if (bulletCount < 30 && leftAmmo > 0)
        {
            return true;
        }

        return false;
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
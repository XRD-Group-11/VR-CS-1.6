using System;
using UnityEngine;

public class Slide : MonoBehaviour
{
    [SerializeField] private float limit = 0.01f;
    [SerializeField] private Gun gun;
    
    private float slider = 0f;
    private Vector3 startPoisition;

    private void Awake()
    {
        startPoisition = transform.localPosition;
    }

    public void Trigger()
    {
        AudioManager.instance.Play("ak47_boltpull");
        slider = 1f;
        gun.BoltPull();
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(startPoisition, startPoisition + (-Vector3.forward * limit), slider);

        slider -= Time.deltaTime * 4;

        slider = Math.Clamp(slider, 0, 1f);
    }
}

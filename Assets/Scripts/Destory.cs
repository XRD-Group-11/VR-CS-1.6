using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destory : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    void Update()
    {

        if(duration <= 0)
            Destroy(this.gameObject);
        duration -= Time.deltaTime;
    }
}

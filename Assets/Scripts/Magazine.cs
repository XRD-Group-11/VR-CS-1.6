using System;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    public int ammo = 30;

    public void TakeOneBullet()
    {
        ammo--;
        ammo = Math.Clamp(ammo, 0, Int32.MaxValue);
    }
}

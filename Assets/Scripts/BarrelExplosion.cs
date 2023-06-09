﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplosion : MonoBehaviour
{
    public GameObject explosion;
    public GameObject barrel;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerProjectile")
        {
            barrel.SetActive(false);
            explosion.SetActive(true);
            if (gameObject.name.Contains("Water")) { SoundManager.PlaySound(SoundManager.Sound.WaterBarrel, 1f);}
            else if (gameObject.name.Contains("Exploding")) { SoundManager.PlaySound(SoundManager.Sound.Barrel, 1f); }

        }
    }
}

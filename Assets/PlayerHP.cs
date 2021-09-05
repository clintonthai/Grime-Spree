﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public GameObject Player;
    public Slider healthBar;
    private Health health;

    // Start is called before the first frame update
    void Start()
    {
        health = Player.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.maxValue = health.GetMaxHealth();
        healthBar.value = health.GetHealth();
        
    }

}

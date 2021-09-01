﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthAddon : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1f;
    private float currentHealth = 1f;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //update UI here
    }

    public void ChangeHealth (float delta) 
    {   
        currentHealth += delta;

        if (currentHealth > maxHealth) {
            currentHealth = maxHealth;
        }
    }

    public float GetHealth ()
    {
        return currentHealth;
    }
}

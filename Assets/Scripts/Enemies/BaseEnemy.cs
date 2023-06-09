﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class BaseEnemy : Upgradeable
{
    [HideInInspector] public bool CanAct = false;

    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb2d;
    public Health health { get; protected set; }
    public Hitbox hitbox;
    

    [Header("Optional Components")]
    public GameObject floorMarker; // enemy will not necessarily have one
    public Animator animator;
    public SpriteFlash spriteFlash;
    [HideInInspector] public PathNavigator navigator; // enemy will not necessarily have one
    [HideInInspector] public PlayerController player;

    public delegate void EnemyDelegate (BaseEnemy e);

    public EnemyDelegate onDeath;

    [Header("Parameters")]
    [SerializeField] protected float invincibilityTime = 0.2f;
    protected float invincibilityTimer;
    [SerializeField] protected List<string> actions; // daniel, refactor to use this when you need to
    //Note that this delegate doesn't take any parameters, though that can defo change.
    public delegate IEnumerator EnemyActionDelegate();
    protected Dictionary<string, EnemyActionDelegate> actionTable;
   

    protected virtual void Awake() {    
        navigator = GetComponent<PathNavigator>();
    }

    protected virtual void OnEnable() {
        PlayerController.OnDeath += OnPlayerDied;
    }
    protected virtual void OnDisable() {
        PlayerController.OnDeath -= OnPlayerDied;
    }

    protected virtual void Start() {
        GetBaseProps();

        health = GetComponent<Health>();
        rb2d = GetComponent<Rigidbody2D>();

        actionTable = new Dictionary<string, EnemyActionDelegate>();
        var classType = this.GetType();

        hitbox.OnTriggerEnter.AddListener(OnEnterHazard);

        //Make action table
        foreach (string s in actions) {
            if (s == "" || Char.IsLower (s[0])) {
                Debug.LogWarning("Enemy " + gameObject.name + " has an invalid action name");
                continue;
            }
            //Get action method
            var m = classType.GetMethod("Action_" + s, BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            if (m != null) {
                // Add to the action table. You can call the method by:
                // actionTable["Test"]();
                actionTable.Add(s, (EnemyActionDelegate) m.CreateDelegate(typeof (EnemyActionDelegate), this));
            }
        }
    }


    public virtual void OnInit() {
        // doesn't need to do anything right now
        // roommanager has already set our player, pathfinding, and canAct bool
    }


    protected virtual void FixedUpdate() {
        invincibilityTimer = Mathf.MoveTowards(invincibilityTimer, 0f, Time.deltaTime);
    }

    protected virtual void OnPlayerDied(PlayerController player) {
        navigator?.Stop();
        CanAct = false;
    }



    protected virtual void OnEnterHazard(Collider2D otherCollider) {
        otherCollider.GetComponentInParent<BaseProjectile>()?.OnHitEntity();

        if (invincibilityTimer == 0f) {
            // take a hit
            TakeDamage(otherCollider);
        }
    }

    protected virtual void TakeDamage(Collider2D other) {
        int hitAmount = 1;
        var damage = other.GetComponent<Damage>();
        if (damage != null) {
            hitAmount = damage.damage;
        }
        else {
            Debug.LogError (other.gameObject.name + " is doing damage for some reason!");
        }
        health.ChangeHealth(-hitAmount);
        if(this.CompareTag("Boss1")) {SoundManager.PlaySound(SoundManager.Sound.TurretBossDamage, 0.3f); }

        invincibilityTimer = invincibilityTime;
        spriteFlash?.Flash(invincibilityTime, other.transform.position.x - transform.position.x);

        if (health.GetHealth() <= 0) {
            CanAct = false;

            // i call the deathevent here instead of in Die() because i expect subclasses to not call base.Die()
            // (since the default implementation destroys the object with no animation)
            onDeath?.Invoke(this);
            onDeath = null;

            Die();
        }
    }

    protected virtual void Die() {
        Destroy(gameObject);
    }



    protected IEnumerator Action_Test () {
        Debug.Log("Action test pass");
        yield return 0;
    }
}

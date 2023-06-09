﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpongeGunAimLine : MonoBehaviour
{
    public float spongeWidth;
    public float LineLength { get; private set; }

    private LineRenderer lineRenderer;
    public WeaponSystem weaponSystem;


    private void Start() {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        if (Time.deltaTime == 0) {
            return;
        }

        lineRenderer.SetPosition(1, Vector3.right * LineLength);

        if (weaponSystem != null) {
            lineRenderer.enabled = weaponSystem.weapon.CanFire(weaponSystem._ammo);
        }
    }

    private void FixedUpdate() {
        if (Time.deltaTime == 0) {
            return;
        }
        
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            Vector2.one * spongeWidth,
            transform.rotation.eulerAngles.z,
            transform.right,
            50f,
            LayerMask.GetMask("Wall")
        );

        LineLength = hit.collider != null ? hit.distance : 50f;
    }
}

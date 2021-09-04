﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapCollider2D))]
public class TileMapController : MonoBehaviour
{
    private Tilemap tm;
    public Tile[] tiles;
    // Start is called before the first frame update
    void Start()
    {
        tm = this.GetComponent<Tilemap>();
        var bounds = tm.cellBounds;
        foreach (var cell in bounds.allPositionsWithin) {
            // do this later
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay2D (Collider2D col) {
        // Debug.Log (col.gameObject.name);
        if (col.gameObject.tag == "PlayerProjectile") {
            Vector3 p = col.gameObject.transform.position;
            Vector3Int pos = tm.WorldToCell(p);
            TileBase c_tile = tm.GetTile(pos);
            if (c_tile != null) {
                tm.SetTile(pos, tiles[0]);
            }

            //Gets 
            Vector3 min = col.bounds.min;
            Vector3 max = col.bounds.max;
            for (float x_min = min.x; x_min < max.x; x_min+=tm.cellSize.x) {
                for (float y_min = min.y; y_min < max.y; y_min+=tm.cellSize.y) {
                    Vector3 point = new Vector3(x_min, y_min,0);
                    var otherClosestPoint = col.ClosestPoint(point);
                    Vector3Int otherClosestCell = tm.WorldToCell(otherClosestPoint);
                    pos = tm.WorldToCell(point);
                    if (otherClosestCell != pos) {
                        continue;
                    }
                    c_tile = tm.GetTile(pos);
                    if (c_tile != null) {
                        tm.SetTile(pos, tiles[0]);
                    }
                }
            }            
        }

        
        // if (col.gameObject.tag == "PlayerProjectile") {
        //     if (health != null) {
        //         health.ChangeHealth(-1);
        //         float per = (1 - health.GetHealthPercent())/2 + health.GetHealthPercent();
        //         this.transform.localScale = new Vector3(per,per,per);
        //     }
        //     Destroy(col.gameObject);
        // }
        // Debug.Log("hi there");

        // if (health.GetHealth() <= 0) {
        //     Destroy(gameObject);
        // }
    }
}

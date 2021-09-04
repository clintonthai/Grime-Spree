﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorMarkerData
{
    public FloorMarker floorMarker;
    public HashSet<Vector3Int> previousPositions;

    public FloorMarkerData(FloorMarker fm) {
        floorMarker = fm;
        previousPositions = new HashSet<Vector3Int>();
    }
}


[RequireComponent(typeof(TilemapCollider2D))]
public class FloorController : MonoBehaviour
{
    private Tilemap tm;
    public Tile[] tiles;

    private Dictionary<Collider2D, FloorMarkerData> floorMarkers;


    // Start is called before the first frame update
    void Start()
    {
        floorMarkers = new Dictionary<Collider2D, FloorMarkerData>();

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

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.tag == "FloorMarker") {
            FloorMarker floorMarker = col.GetComponent<FloorMarker>();
            if (floorMarker != null) {
                // track this object
                floorMarkers.Add(col, new FloorMarkerData(floorMarker));
            }
        }
    }

    void OnTriggerStay2D (Collider2D col) {
        if (floorMarkers.ContainsKey(col)) {
            HashSet<Vector3Int> cells = new HashSet<Vector3Int>();

            // loop over the collider's bounds to see what tiles it might be in
            Vector3Int min = tm.WorldToCell(col.bounds.min);
            Vector3Int max = tm.WorldToCell(col.bounds.max);
            for (int x = min.x; x <= max.x; x++) {
                for (int y = min.y; y <= max.y; y++) {
                    Vector3Int cell = new Vector3Int(x, y, 0);

                    // check if this position actually has a valid tile on it
                    var c_tile = tm.GetTile(cell);
                    if (c_tile == null || !c_tile.name.StartsWith("Dirty")) {
                        continue;
                    }

                    // check if the collider is actually touching this cell
                    var otherClosestPoint = col.ClosestPoint(tm.CellToWorld(cell));
                    Vector3Int otherClosestCell = tm.WorldToCell(otherClosestPoint);
                    if (cell != otherClosestCell) {
                        continue;
                    }

                    // this is a valid position that we should consider
                    cells.Add(cell);
                    if (floorMarkers[col].previousPositions.Contains(cell)) {
                        // the collider was on this tile before, so let's not change this tile again.
                        continue;
                    }

                    // deal "damage" to the floor
                    if (!int.TryParse(c_tile.name.Substring("Dirty".Length), out int tileHealth)) {
                        Debug.LogError("The floor tile " + c_tile.name + " does not look like 'DirtyX' where X is a number");
                        continue;
                    }
                    tileHealth -= floorMarkers[col].floorMarker.markAmount;
                    Debug.Log("setting " + cell + " to Dirty" + tileHealth);
                    tm.SetTile(cell, tiles[Mathf.Clamp(tileHealth, 0, tiles.Length - 1)]);
                }
            }    

            floorMarkers[col].previousPositions = cells;        
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        floorMarkers.Remove(other);
    }
}

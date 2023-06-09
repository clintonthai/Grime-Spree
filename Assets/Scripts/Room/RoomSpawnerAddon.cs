using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;


[RequireComponent(typeof(RoomManager))]
public class RoomSpawnerAddon : MonoBehaviour
{
    public EnemyTypesSO enemyTypesSO;
    private Dictionary<string, EnemyTypesSO.EnemyType> enemyTypes;

    // public AudioSource spawnSound;
    public int spawnAreaCount = 3;
    public float spawnTimer = 15f;
    private float m_spawnTimer;
    private RoomManager rm;

    public GameObject[] glist;
    public string[] enemyTypesToSpawn;
    public GameObject enemySpawnPrefab;

    private Tilemap overlay;

    public GameObject spawnMarkerPrefab;

    void Start ()
    {
        enemyTypes = enemyTypesSO.GetEnemyTypes();
        
        m_spawnTimer = spawnTimer;
        rm = this.GetComponent<RoomManager>();
        /*
        var generatedFloor = new GameObject ("dirty spawn marker overlay");
        generatedFloor.transform.SetParent (rm.dirtyTiles.transform.parent);
        generatedFloor.transform.localPosition = Vector3.zero;

        var tm = generatedFloor.AddComponent<Tilemap>();
        tm.tileAnchor = new Vector3(0.5f,0.5f,0);
        var tmr = generatedFloor.AddComponent<TilemapRenderer>();
        tmr.sortingLayerName = "Floor";
        tmr.sortingOrder = 3;
        overlay = tm;

        if (tileToFlicker == null) 
        {
            Debug.LogWarning ("Flicker tile for " + rm.gameObject.name + " is null! replacing with solid red");
            Texture2D red = new Texture2D (1, 1, TextureFormat.RGBA32, 1, true);
            red.SetPixel(0,0, Color.red);
            red.Apply(true);

            tileToFlicker = ScriptableObject.CreateInstance<Tile>();                
            tileToFlicker.sprite = Sprite.Create(red, new Rect(0,0,1,1), Vector2.one / 2f, 1);
        }
        */

        rm.onRoomClear += (a, b) => {
            StopAllCoroutines();
            // Destroy (generatedFloor);
        };

    }

    void FixedUpdate ()
    {
     
        m_spawnTimer -= Time.fixedDeltaTime;
        if (m_spawnTimer <= 0 && rm.IsRoomActive) {
            m_spawnTimer = spawnTimer;
            int retry = 5;
            for (int i = 0; i < spawnAreaCount - rm.NumEnemy; i++) {
                if (!AttemptSpawn () && retry > 0) {
                    i--;
                    retry -= 1;
                }
                
            }
        }
    }

    private bool AttemptSpawn ()
    {
        var min = rm.dirtyTiles.Min;
        var max = rm.dirtyTiles.Max;
        var rx = Random.Range(min.x, max.x);
        var ry = Random.Range(min.y, max.y);
        if (rm.dirtyTiles.IsTileDirty(new Vector2Int(rx,ry), .8f)) {
            int m = enemyTypesToSpawn.Length;
            var g = enemyTypesToSpawn[Random.Range(0, m)];

            /*
            var mark = new Marker (rx,ry, 5f, g, tileToFlicker);
            StartCoroutine(mark.Begin(overlay,rm));
            // spawnSound.Play(); 
            //Debug.Log("Halo2");
            */
            rm.PrepareForEnemy();
            GameObject created = Instantiate(
                enemySpawnPrefab,
                rm.dirtyTiles.tm.GetCellCenterWorld(new Vector3Int(rx, ry, 0)),
                Quaternion.identity,
                rm.enemiesContainer           
            );
            EnemySpawnMarker enemySpawn = created.GetComponent<EnemySpawnMarker>();
            enemySpawn.SetEnemy(enemyTypes[g].prefab, rm);
            SoundManager.PlaySound(SoundManager.Sound.EnemySpawn, 0.5f);
            
            return true;
        }
        else {
            return false;
        }
    }

    private class Marker 
    {
        public readonly int x;
        public readonly int y;
        public AudioSource sp;
        // private RoomSpawnerAddon ad = new RoomSpawnerAddon();

        //Time from marker creation to spawning enemy
        public float m_timer;

        public readonly GameObject enemy;
        
        public Marker (int x, int y, float timer, GameObject obj) 
        {
            this.x = x;
            this.y = y;
            this.m_timer = timer;
            this.enemy = obj;
        }

        public IEnumerator Begin (Tilemap tm, RoomManager rm)
        {

            var count = 0;
            while (m_timer >= 0 && tm != null && rm.dirtyTiles.IsTileDirty(new Vector2Int(x,y), 0.2f)) {

                count ++;

                m_timer -= 0.5f;
                

                yield return new WaitForSeconds(0.5f);

            }
            
            if (rm.dirtyTiles.IsTileDirty(new Vector2Int(x,y), 0.2f)) 
            {
                // var rmnumber = GameObject.Find("Room (7)");
                // sp = rmnumber.GetComponent<RoomSpawnerAddon>().spawnSound;
                //  sp.Play();

                yield return new WaitForSeconds(0.5f);

                GameObject created = Instantiate(enemy, tm.CellToWorld(new Vector3Int(x,y,0)), Quaternion.identity, rm.enemiesContainer);
                rm.InitEnemy(created.transform);
            }

            yield return 0;
        }
    }
}
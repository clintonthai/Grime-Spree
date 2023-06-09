using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

[RequireComponent(typeof(RoomManager))]
public class RoomWaveAddon : MonoBehaviour
{
    // room manager, lazily initialized so that it is available at editor time
    private RoomManager _roomManager;
    public RoomManager roomManager => _roomManager ?? (_roomManager = GetComponent<RoomManager>());

    public EnemyTypesSO enemyTypesSO;
    private Dictionary<string, EnemyTypesSO.EnemyType> enemyTypes;

    public GameObject spawnMarkerPrefab;
    public BaseEnemy bossEnemy;

    [SerializeField] private Wave[] waves;

    void Start()
    {
        enemyTypes = enemyTypesSO.GetEnemyTypes();

        // StartCoroutine (WaitForRoom ());
        roomManager.roomSpecificOnEnter += WaitForRoom;

        roomManager.onRoomReset += (a, b) => {
            StopAllCoroutines();
            // StartCoroutine (WaitForRoom ());
        };
    }

    public void WaitForRoom (PlayerController a, RoomManager r)
    {
        // yield return new WaitUntil (() => roomManager.IsRoomActive);

        var tm = roomManager.dirtyTiles.gameObject.GetComponent<Tilemap>();
        roomManager.cleanUIPrefab?.Clear();
        foreach (var wave in waves) {
            foreach (var waveSpawn in wave.spawns) {
                roomManager.cleanUIPrefab?.AddWave(wave.thresh);
                StartCoroutine(SetWaveSpawn(waveSpawn, wave.thresh, tm));
            }
        }
        
    }

    public IEnumerator SetWaveSpawn (WaveSpawn waveSpawn, float thresh, Tilemap tm)
    {
        if (!enemyTypes.ContainsKey(waveSpawn.enemy)) {
            Debug.LogError("Room " + gameObject + " wants to spawn a \"" + waveSpawn.enemy + "\" enemy, but there is no such enemy!");
            yield break;
        }

        yield return new WaitUntil(() => {
            // if there is a boss, we will base spawns off of the boss health
            // otherwise, we use the level of cleanliness in the room
            if (bossEnemy != null) {
                return 1f - bossEnemy.health.GetHealthPercent() >= thresh;
            }
            else {
                return roomManager.dirtyTiles.GetCleanPercent() >= thresh;
            }
        });

        roomManager.PrepareForEnemy();
        yield return new WaitForSeconds(waveSpawn.delay);
        
        GameObject created = Instantiate(
            spawnMarkerPrefab,
            WaveSpawn.XYToPosition(waveSpawn.xcoord, waveSpawn.ycoord, tm),
            Quaternion.identity,
            roomManager.enemiesContainer           
        );
        created.GetComponent<EnemySpawnMarker>().SetEnemy(enemyTypes[waveSpawn.enemy].prefab, roomManager);
        SoundManager.PlaySound(SoundManager.Sound.EnemySpawn, 0.5f);
    }
    
    [System.Serializable]
    public struct WaveArray
    {
        public Wave[] waves;
        public WaveArray(Wave[] w) {
            waves = w;
        }
    }

    [System.Serializable]
    public struct Wave
    {
        public float thresh;
        public WaveSpawn[] spawns;
    }

    [System.Serializable]
    public struct WaveSpawn
    {
        public float xcoord;
        public float ycoord;
        public string enemy;
        public float delay;

        public static Vector3 XYToPosition(float xcoord, float ycoord, Tilemap tm) {
            // lerp between floored and ceiled tile positions
            var lo = tm.GetCellCenterWorld(new Vector3Int(Mathf.FloorToInt(xcoord), Mathf.FloorToInt(ycoord), 0));
            var hi = tm.GetCellCenterWorld(new Vector3Int(Mathf.CeilToInt(xcoord), Mathf.CeilToInt(ycoord), 0));
            return new Vector3(Mathf.Lerp(lo.x, hi.x, xcoord.Mod(1)), Mathf.Lerp(lo.y, hi.y, ycoord.Mod(1)));
        }

        public static float PositionToX(Vector3 worldPos, Tilemap tm) {
            var cell = tm.WorldToCell(worldPos);
            var cellWorld = tm.GetCellCenterWorld(cell);

            // get the cell size in the hackiest way possible
            var cellSize = tm.GetCellCenterWorld(new Vector3Int(cell.x + 1, cell.y + 1, 0)) - cellWorld;
            
            return (float)cell.x + (worldPos.x - cellWorld.x) % cellSize.x;
        }

        public static float PositionToY(Vector3 worldPos, Tilemap tm) {
            var cell = tm.WorldToCell(worldPos);
            var cellWorld = tm.GetCellCenterWorld(cell);

            // get the cell size in the hackiest way possible
            var cellSize = tm.GetCellCenterWorld(new Vector3Int(cell.x + 1, cell.y + 1, 0)) - cellWorld;

            return (float)cell.y + (worldPos.y - cellWorld.y) % cellSize.y;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ItemTypesScriptableObject", order = 1)]
public class ItemTypesSO : ScriptableObject
{
    [System.Serializable]
    public class Item {
        public string name;
        public GameObject prefab;
        public int shopPrice;

        public Item(string n, GameObject p, int price) {
            name = n;
            prefab = p;
            shopPrice = price;
        }
    }

    [SerializeField] private Item[] itemTypes;

    public Dictionary<string, GameObject> GetItemDict() {
        var dict = new Dictionary<string, GameObject>();
        foreach (Item e in itemTypes) {
            dict.Add (e.name, e.prefab);
        }
        return dict;
    }

    public int GetPrice (string name) {
        foreach (var item in itemTypes)
        {
            if (item.name == name) {
                return item.shopPrice;
            }
        }
        return 0;
    }
}

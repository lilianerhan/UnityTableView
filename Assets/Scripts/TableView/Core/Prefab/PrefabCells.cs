using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TableView
{
    public class PrefabCells
    {
        private Dictionary<string, GameObject> registeredCells;

        public PrefabCells()
        {
            registeredCells = new Dictionary<string, GameObject>();
        }

        public void RegisterPrefabForCellReuseIdentifier(GameObject prefab, string cellReuseIdentifier)
        {
            if (string.IsNullOrEmpty(cellReuseIdentifier)) return;
            if (registeredCells.ContainsKey(cellReuseIdentifier)) return;

            registeredCells.Add(cellReuseIdentifier, prefab);
        }

        public GameObject PrefabForCellReuseIdentifier(string cellReuseIdentifier)
        {
            if (string.IsNullOrEmpty(cellReuseIdentifier)) return null;
            if (!IsRegisteredCellReuseIdentifier(cellReuseIdentifier)) return null;

            return registeredCells[cellReuseIdentifier];
        }

        public bool IsRegisteredCellReuseIdentifier(string cellReuseIdentifier)
        {
            if (string.IsNullOrEmpty(cellReuseIdentifier)) return false;

            return registeredCells.ContainsKey(cellReuseIdentifier);
        }
    }
}
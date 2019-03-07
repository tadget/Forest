namespace Tadget.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Tile Objects", menuName = "Map/Tile Objects", order = 1)]
    public class TileObjects : ScriptableObject
    {
        [SerializeField]
        private List<GameObject> objects;
        private Dictionary<string, GameObject> objectMap;

        public void Init()
        {
            objectMap = new Dictionary<string, GameObject>();
            if (objects == null)
                return;
            foreach (var obj in objects)
            {
                if (objectMap.ContainsKey(obj.name))
                {
                    Debug.LogWarningFormat("Trying to add duplicate name {0} to object map. Ensure all objects have a unique name.", obj.name);
                }
                else
                {
                    objectMap.Add(obj.name, obj);
                }
            }
        }

        public bool TryGetObject(string name, out GameObject go)
        {
            go = null;
            return objectMap.TryGetValue(name, out go);
        }
    }
}

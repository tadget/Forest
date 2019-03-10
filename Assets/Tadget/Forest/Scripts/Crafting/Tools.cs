namespace Tadget
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public static class Tools
    {
        public static void SetLayerRecursively(GameObject go, int layerNumber)
        {
            if (go == null) return;
            foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
                trans.gameObject.layer = layerNumber;
        }

        public static void SetTriggerRecursively(GameObject go, bool isTrigger)
        {
            if (go == null) return;
            foreach (Collider coll in go.GetComponentsInChildren<Collider>(true))
                coll.isTrigger = isTrigger;
        }

        public static void SetPhysicMaterialRecursively(GameObject go, PhysicMaterial material)
        {
            if (go == null) return;
            foreach (Collider coll in go.GetComponentsInChildren<Collider>(true))
                coll.material = material;
        }

        public static Dictionary<Renderer, Material[]> GetRendererMaterialDict(GameObject go)
        {
            Dictionary<Renderer, Material[]> rendererMaterialDict = new Dictionary<Renderer, Material[]>();
            if (go == null) return rendererMaterialDict;
            foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>(true))
                rendererMaterialDict.Add(renderer, renderer.materials);
            return rendererMaterialDict;
        }

        public static void SetRendererMaterials(Renderer[] renderers, Material mat)
        {
            foreach (var renderer in renderers)
            {
                Material[] newMats = new Material[renderer.materials.Length];
                newMats.Fill(mat);
                renderer.materials = newMats;
            }
        }

        public static void SetRendererMaterials(Dictionary<Renderer, Material[]> rendererMaterialDict)
        {
            foreach (var pair in rendererMaterialDict)
                pair.Key.materials = pair.Value;
        }
    }
}

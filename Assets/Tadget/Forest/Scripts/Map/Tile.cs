using UnityEditor;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Serialization;

namespace Tadget.Map
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    [CreateAssetMenu(fileName = "Tile", menuName = "Map/Tile", order = 1)]
    public class Tile : ScriptableObject
    {
        [ValueDropdown("validTypes")]
		public Type type;

        #pragma warning disable 0414
        private Type[] validTypes = new[]
        {
            Type.CABIN,
            Type.YARD,
            Type.OUTER,
            Type.BIOME
        };
        #pragma warning restore 0414

		public enum Type
		{
			UNKNOWN = -1,
			CABIN = 0,
			YARD = 1,
			OUTER = 2,
			BIOME = 3
		}

        [System.Serializable]
        public class ObjectList
        {
            [LabelText("$name")]
            [PreviewField(70, ObjectFieldAlignment.Left)]
            [OnValueChanged("OnSelectedNewObject")]
            [Required("A GameObject must be selected")]
            public GameObject go;

            #if UNITY_EDITOR
            private bool AssetFilter(GameObject _go)
            {
                return AssetDatabase.GetAssetPath(_go).Contains(".prefab");
            }
            #endif

            #pragma warning disable 0414
            private string name;
            #pragma warning restore 0414
            private void OnSelectedNewObject(GameObject go)
            {
                name = go != null ? go.name : "null";
            }
        }

        [System.Serializable]
        public class Object
        {
            private ObjectList AddObjectList()
            {
                return (objectGroup.Count > 0 && objectGroup[objectGroup.Count - 1] != null)
                    ? (ObjectList)Sirenix.Serialization.SerializationUtility.CreateCopy(objectGroup[objectGroup.Count - 1])
                    : new ObjectList();
            }

            [ListDrawerSettings(CustomAddFunction = "AddObjectList")]
            [ValidateInput("IsListPopulated", "An Object Group must have at least 1 element")]
            public List<ObjectList> objectGroup;

            #pragma warning disable 0414
            private string group1 = "Spawning Options";
            #pragma warning restore 0414

            public bool IsListPopulated(List<ObjectList> list)
            {
                return list != null && list.Count > 0;
            }

            public bool IsListPopulated()
            {
                return objectGroup != null && objectGroup.Count > 0;
            }

            [Title("Position")]
            [ShowIf("IsListPopulated")]
            [FoldoutGroup("$group1")]
            public bool isPositionRandomized = true;

            [ShowIf("isPositionRandomized")]
            [ShowIf("IsListPopulated")]
            [FoldoutGroup("$group1")]
            [MinMaxSlider(-10, 10)]
            public Vector2 minMaxOffset = new Vector2(-10, 10);

            [HideIf("isPositionRandomized")]
            [ShowIf("IsListPopulated")]
            [FoldoutGroup("$group1")]
            [ValidateInput("OnPositionRandomized", "At least 1 fixed position must be provided")]
            public List<Vector3> positions;

            private bool OnPositionRandomized(List<Vector3> ps)
            {
                return isPositionRandomized || (!isPositionRandomized && ps.Count > 0);
            }

            [Title("Rotation")]
            [ShowIf("IsListPopulated")]
            [FoldoutGroup("$group1")]
            public bool isRotationRandomized = true;

            [Title("Count")]
            [ShowIf("IsListPopulated")]
            [FoldoutGroup("$group1")]
            public bool isFixedCount = true;

            [ShowIf("isFixedCount")]
            [ShowIf("IsListPopulated")]
            [PropertyRange(0,30)]
            [FoldoutGroup("$group1")]
            public int count = 1;

            [HideIf("isFixedCount")] [ShowIf("IsListPopulated")] [FoldoutGroup("$group1")] [MinMaxSlider(0, 30)]
            public Vector2Int minMaxRandomCount;

            [Title("Chance")]
            [ShowIf("IsListPopulated")]
            [FoldoutGroup("$group1")]
            public bool isRandomChanceToSpawn = false;

            [ShowIf("isRandomChanceToSpawn")]
            [ShowIf("IsListPopulated")]
            [FoldoutGroup("$group1")]
            [Range(0,1)]
            public float chance;
        }

        public List<Object> objects;
        #if UNITY_EDITOR
        private GameObject previewTile;

        [Button(ButtonSizes.Large)] [PropertyOrder(0)]
        private void CreatePreviewTile()
        {
            if(previewTile)
                DestroyImmediate(previewTile);
            previewTile = TileFactory.Create(this, Vector3.zero,null);
        }

        [Button(ButtonSizes.Large)]
        private void FocusOnTile()
        {
            var isLocked = ActiveEditorTracker.sharedTracker.isLocked;
            if(!isLocked)
                ActiveEditorTracker.sharedTracker.isLocked = true;

            Selection.activeGameObject = previewTile.gameObject;
            SceneView.lastActiveSceneView.FrameSelected();

            ActiveEditorTracker.sharedTracker.isLocked = isLocked;
        }

        [Button(ButtonSizes.Large)] [PropertyOrder(0)]
        private void DestroyPreviewTile()
        {
            if(previewTile)
                DestroyImmediate(previewTile);
        }
        #endif

    }
}

namespace Tadget
{
    using System;
    using UnityEngine;
    using System.Collections;
    using Sirenix.OdinInspector;

    [RequireComponent(typeof(Transform))]
    public class SpawnComponent : ActionComponent
    {
        [AssetsOnly, PreviewField(ObjectFieldAlignment.Left, Height = 70)]
        public GameObject objToSpawn;

        public bool tryToFindAParent;
        [ShowIf("tryToFindAParent")]
        public LayerMask layer;

        public override void Use(Action Complete)
        {
            var go = GameObject.Instantiate(
                objToSpawn,
                transform.position,
                objToSpawn.transform.rotation);

            if (tryToFindAParent)
                go.transform.parent = TryGetParent();

            Complete();
        }

        private Transform TryGetParent()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, layer))
                return hit.transform;
            else
                return null;
        }
    }
}

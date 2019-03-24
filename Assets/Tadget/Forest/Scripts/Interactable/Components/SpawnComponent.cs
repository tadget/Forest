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

        public override void Use(Action Complete)
        {
            GameObject.Instantiate(
                objToSpawn,
                transform.position,
                transform.rotation);

            Complete();
        }
    }
}

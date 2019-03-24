namespace Tadget
{
    using System;
    using UnityEngine;

    public class DestroyComponent : ActionComponent
    {
        public GameObject objToDestroy;

        public override void Use(Action Complete)
        {
            #if UNITY_EDITOR
            GameObject.DestroyImmediate(objToDestroy);
            #else
            GameObject.Destroy(objToDestroy);
            #endif
            Complete();
        }
    }
}

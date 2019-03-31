using Sirenix.OdinInspector;

namespace Tadget
{
    using System;
    using UnityEngine;

    public class DestroyComponent : ActionComponent
    {
        public bool destroySelf;
        [HideIf("destroySelf")]
        public GameObject objToDestroy;

        public override void Use(Action Complete)
        {
            if (destroySelf) Complete();
            #if UNITY_EDITOR
            GameObject.DestroyImmediate(destroySelf ? gameObject : objToDestroy);
            #else
            GameObject.Destroy(destroySelf ? gameObject : objToDestroy);
            #endif
            if (!destroySelf) Complete();
        }
    }
}

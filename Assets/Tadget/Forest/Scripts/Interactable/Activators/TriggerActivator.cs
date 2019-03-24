namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class TriggerActivator : ActionActivator
    {
        [ValueDropdown("GetTags", IsUniqueList = true, DropdownTitle = "Select Tag", ExcludeExistingValuesInList = true)]
        public List<string> activationTags;

        #if UNITY_EDITOR
        private IEnumerable GetTags()
        {
            return UnityEditorInternal.InternalEditorUtility.tags;
        }
        #endif

        private new void Awake()
        {
            base.Awake();
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (activationTags.Contains(other.tag))
            {
                Activate();
            }
        }
    }
}

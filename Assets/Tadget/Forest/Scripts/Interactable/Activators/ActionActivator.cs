namespace Tadget
{
    using System;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using System.Collections;
    using System.Linq;

    [DisallowMultipleComponent]
    public abstract class ActionActivator : MonoBehaviour
    {
        [SerializeField, ValidateInput("RequireActionComponents", "At least 1 Action Component is required"), InlineEditor(), PropertyOrder(50)]
        private ActionComponent[] actionComponents;
        [SerializeField, EnumToggleButtons]
        private ActivationMode activationMode;
        [SerializeField, Range(0, Int16.MaxValue), ShowIf("activationMode", ActivationMode.Finite)]
        private int finiteActivationCount = 1;
        [SerializeField, Range(0, Int16.MaxValue)]
        private readonly float cooldown = 1f;
        private bool busy;
        private bool waiting;

        public enum ActivationMode
        {
            Finite,
            Unlimited
        }

        protected void Awake()
        {
            busy = false;
            waiting = false;
        }

        [Button("Activate"), PropertyOrder(100), HideInEditorMode]
        protected void Activate()
        {
            if (busy) return;
            else if (activationMode == ActivationMode.Finite)
            {
                if (finiteActivationCount < 1)
                    return;
                else
                    finiteActivationCount--;
            }
            StartCoroutine(ActivateCoroutine());
        }

        private IEnumerator ActivateCoroutine()
        {
            busy = true;
            yield return UseActionComponents();
            yield return new WaitForSeconds(cooldown);
            busy = false;
        }

        private IEnumerator UseActionComponents()
        {
            if (actionComponents != null)
            {
                for (int i = 0; i < actionComponents.Length; i++)
                {
                    if(actionComponents[i] == null) continue;
                    waiting = true;
                    actionComponents[i].Use(UseCompleteCallback);
                    yield return new WaitWhile(() => waiting);
                    yield return new WaitForSeconds(actionComponents[i].delay);
                }
            }
        }

        private void UseCompleteCallback()
        {
            waiting = false;
        }

        private bool RequireActionComponents(ActionComponent[] list)
        {
            return list != null && list.Length > 0 && !list.Contains(null);
        }
    }
}

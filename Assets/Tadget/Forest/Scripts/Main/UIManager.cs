namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        public Text positionDisplay;
        public Text stateDisplay;
        public Camera uiCamera;

        private void OnValidate()
        {
            Debug.Assert(positionDisplay != null, "Position display Text not assigned.");
            Debug.Assert(stateDisplay != null, "State display Text not assigned.");
            Debug.Assert(uiCamera != null, "UI camera not assigned.");

        }

        public void ToggleUICamera(bool on)
        {
            if(uiCamera)
                uiCamera.gameObject.SetActive(on);
        }
    }
}

namespace Tadget
{
    using UnityEngine;
    using UnityEngine.UI;

    public class Interactable : MonoBehaviour
    {
        GameObject playerCam;

        public GameObject interactableUI;
        public Image timerBar;
        public Text timer;

        public float maxDistance = 5f;

        public float timeToOpen = 2f;
        float timeToOpenCounter;

        private void Start()
        {
            InitializeUI();
            playerCam = GameObject.FindGameObjectWithTag("MainCamera");
        }

        private void Update()
        {
            CheckForInteractables("Interactable", maxDistance);
        }

        void Interact()
        {

        }

        /// <summary>
        ///     Check for interactable objects through the camera
        /// </summary>
        /// <param name="layerMask"> Name of the layer mask for interactable objects </param>
        /// <param name="maxDistance"> The maximum distance to which the ray will reach for </param>
        void CheckForInteractables(string layerMask, float maxDistance)
        {
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, LayerMask.GetMask(layerMask)))
            {
                TurnUIVisible(true);
                CheckForControls();
            }
            else
            {
                TurnUIVisible(false);
                ResetTime();
            }
        }

        /// <summary>
        /// Turn text on/off
        /// </summary>
        /// <param name="visibility">The visibility of the text</param>
        void TurnUIVisible(bool visibility)
        {
            interactableUI.SetActive(visibility);
        }


        /// Check if player is holding onto interactable objects
        void CheckForControls()
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Stationary)
                {
                    timeToOpenCounter -= Time.deltaTime;
                    InitializeUI();

                    if (timeToOpenCounter <= 0)
                    {
                        timeToOpenCounter = 0;
                        TurnUIVisible(false);
                    }
                }
            }
            else
            {
                ResetTime();
            }
        }

        /// Initializing UI text's value
        void InitializeUI()
        {
            timerBar.fillAmount = timeToOpenCounter / timeToOpen;
            timer.text = timeToOpenCounter.ToString("0.00");
        }

        /// Reset time counter to its maximum time
        void ResetTime()
        {
            timeToOpenCounter = timeToOpen;
        }
    }
}

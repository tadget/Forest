namespace Tadget
{
    using UnityEngine;
    using UnityEngine.UI;

    public class Compass : MonoBehaviour
    {
        public RawImage compassImage;
        public Transform player;

        /*private void Start()
        {
            compassImage = GetComponent<RawImage>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }*/

        private void Update()
        {
            compassImage.uvRect = new Rect(player.localEulerAngles.y / 360, 0, 1, 1);
        }
    }
}


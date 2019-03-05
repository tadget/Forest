namespace Tadget
{
    using UnityEngine;

    public class TriggerActivate : MonoBehaviour
    {
        public GameObject whatCanActivate;
        public float delayTime = 1f;
        float delayTimeCounter;

        private void Update()
        {
            delayTimeCounter -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider trigger)
        {
            if (trigger.tag == whatCanActivate.tag)
            {

                if (delayTimeCounter <= 0)
                {
                    GetComponent<Spawn>().Use();
                    delayTimeCounter = delayTime;
                }
            }
        }
    }
}

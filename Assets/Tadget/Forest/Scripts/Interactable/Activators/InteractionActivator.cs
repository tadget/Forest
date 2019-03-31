namespace Tadget
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System.Collections.Generic;

    [RequireComponent(typeof(Collider))]
    public class InteractionActivator : ActionActivator, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private float lastOnPointerDownTime;
        [SerializeField, Range(0, Int16.MaxValue)]
        public float activationTime;
        public AudioClip interactionSound;
        private AudioSource interactionAudioSource;
        private Coroutine timerCoroutine;

        public void OnPointerDown(PointerEventData eventData)
        {
            RaycastHit hit;
            if (!(Physics.Raycast(Camera.main.ScreenPointToRay(EventSystem.current.currentInputModule.input.mousePosition), out hit, 2f)
                  && hit.transform.gameObject == gameObject))
                return;
            if(interactionSound) PlayInteractionSound(true);
            lastOnPointerDownTime = Time.time;
            StartTimer();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(interactionSound) PlayInteractionSound(false);
            ResetTimer();
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnPointerUp(eventData);
        }

        private void StartTimer()
        {
            ResetTimer();
            timerCoroutine = StartCoroutine(TimerCoroutine(activationTime,() =>
            {
                if(interactionSound) PlayInteractionSound(false);
                Activate();
            }));
        }

        private void ResetTimer()
        {
            if (timerCoroutine != null)
            {
                StopCoroutine(timerCoroutine);
                timerCoroutine = null;
            }
        }

        private IEnumerator TimerCoroutine(float time, Action Callback)
        {
            while (Time.time - lastOnPointerDownTime < time)
            {
                if(Input.touchCount > 1)
                {
                    if(interactionSound) PlayInteractionSound(false);
                    ResetTimer();
                }
                yield return new WaitForEndOfFrame();
            }
            if (Callback != null) Callback();
        }

        private void PlayInteractionSound(bool play)
        {
            if (play)
            {
                if (interactionAudioSource == null)
                    interactionAudioSource = gameObject.AddComponent<AudioSource>();
                if (interactionAudioSource.clip == null)
                {
                    interactionAudioSource.clip = interactionSound;
                    interactionAudioSource.loop = true;
                }
                interactionAudioSource.Play();
            }
            else
            {
                if(interactionAudioSource != null)
                    interactionAudioSource.Stop();
            }
        }
    }
}

namespace Tadget
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Tilemaps;
   
    public class EventManager : MonoBehaviour
    {
        private Dictionary<string, Action<object>> eventDictionary;
   
        private static EventManager eventManager;

        public static EventManager instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
   
                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();
                    }
                }
                return eventManager;
            }
        }
   
        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, Action<object>>();
            }
        }

        public static void StartListening<T>(string eventName, Action<T> listener)
        {
            Action<object> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += o => listener((T)o);
   
                //Update the Dictionary
                instance.eventDictionary[eventName] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += o => listener((T)o);
                instance.eventDictionary.Add(eventName, thisEvent);
            }
        }
   
        public static void StopListening<T>(string eventName, Action<object> listener)
        {
            if (eventManager == null) return;
            Action<object> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= o => listener((T)o);
   
                //Update the Dictionary
                instance.eventDictionary[eventName] = thisEvent;
            }
        }
   
        public static void TriggerEvent<T>(string eventName, object eventParam)
        {
            Action<object> thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(eventParam);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
    }
}

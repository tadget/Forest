namespace Tadget
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    public class EventComponent : ActionComponent
    {
        public UnityEvent[] events;

        public override void Use(Action Complete)
        {
            if (events != null)
            {
                for (int i = 0; i < events.Length; i++)
                {
                    if(events[i] != null)
                        events[i].Invoke();
                }
            }

            Complete();
        }
    }
}

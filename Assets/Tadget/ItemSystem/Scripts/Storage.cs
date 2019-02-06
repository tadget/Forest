using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tadget
{
    public class Storage : MonoBehaviour
    {
        public List<Transform> Slots;

        public void DoTheThing(Vector3 touchPoint, Transform hand, Transform item, bool placeCall)
        {
            int closestSlot = -1;

            for (int i = 0; i < Slots.Count; i++) // if available
            {
                if (Slots[i].childCount == 0)
                {
                    closestSlot = i;
                    break;
                }
            }

            if (closestSlot != -1) // hover or place
            {
                for (int i = 0; i < Slots.Count; i++)
                {
                    if (Slots[i].childCount == 0)
                    {
                        if ((touchPoint - Slots[i].position).sqrMagnitude <
                            (touchPoint - Slots[closestSlot].position).sqrMagnitude)
                        {
                            closestSlot = i;
                        }
                    }
                }

                hand.transform.position = Slots[closestSlot].position;
                item.transform.rotation = Quaternion.Lerp(item.transform.rotation, Slots[closestSlot].rotation, Time.deltaTime * 10);
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one * item.GetComponent<Item>().scaleWhenStoreds, Time.deltaTime * 10);

                if (placeCall)
                {
                    item.transform.parent = Slots[closestSlot];
                    item.transform.position = Slots[closestSlot].position;
                    item.transform.rotation = Slots[closestSlot].rotation;
                    item.transform.localScale = Vector3.one * item.GetComponent<Item>().scaleWhenStoreds;
                }
            }
        }
    }
}

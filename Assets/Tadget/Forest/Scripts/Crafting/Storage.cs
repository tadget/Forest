namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Storage : MonoBehaviour
    {
        public List<Transform> slots;

        public bool TrySnapItemToSlot(Vector3 touchPoint, Transform hand, Transform item, bool placeCall)
        {
            int closestSlot = -1;

            for (int i = 0; i < slots.Count; i++) // if available
            {
                if (slots[i].childCount == 0)
                {
                    closestSlot = i;
                    break;
                }
            }

            if (closestSlot != -1) // hover or place
            {
                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].childCount == 0)
                    {
                        if ((touchPoint - slots[i].position).sqrMagnitude <
                            (touchPoint - slots[closestSlot].position).sqrMagnitude)
                        {
                            closestSlot = i;
                        }
                    }
                }

                hand.transform.position = slots[closestSlot].position;
                item.transform.rotation = Quaternion.Lerp(item.transform.rotation, slots[closestSlot].rotation, Time.deltaTime * 10);
                item.transform.localScale = Vector3.Lerp(item.transform.localScale, Vector3.one * item.GetComponent<Item>().scaleWhenStored, Time.deltaTime * 10);

                if (placeCall)
                {
                    item.transform.parent = slots[closestSlot];
                    item.transform.position = slots[closestSlot].position;
                    item.transform.rotation = slots[closestSlot].rotation;
                    item.transform.localScale = Vector3.one * item.GetComponent<Item>().scaleWhenStored;
                }
                return true;
            }

            return false;
        }
    }
}

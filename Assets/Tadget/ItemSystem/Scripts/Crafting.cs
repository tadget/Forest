using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tadget
{
    public class Crafting : MonoBehaviour
    {
        public Storage ItemsOnMe;
        public List<Recipe> recipies;

        public void Check4Craft()
        {
            foreach (Recipe rec in recipies)
            {
                List<int> ItemsOnTable = new List<int>();
                foreach (Transform tr in ItemsOnMe.Slots)
                {
                    if (tr.childCount == 1)
                    {
                        ItemsOnTable.Add(tr.GetChild(0).GetComponent<Item>().id);
                    }
                    else
                    {
                        ItemsOnTable.Add(-1);
                    }
                }

                bool OK = true;

                if (rec.OrderSensitive)
                {
                    if (rec.Ingredients.Count == ItemsOnTable.Count)
                    {
                        for (int i = 0; i < rec.Ingredients.Count; i++)
                        {
                            if (rec.Ingredients[i] != ItemsOnTable[i])
                            {
                                OK = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    ItemsOnTable.RemoveAll(ism1);
                    if (rec.Ingredients.Count == ItemsOnTable.Count)
                    {
                        foreach (int id in rec.Ingredients)
                        {
                            if (!ItemsOnTable.Contains(id))
                            {
                                OK = false;
                                break;
                            }
                            else
                            {
                                ItemsOnTable.Remove(id);
                            }
                        }
                    }
                    else
                    {
                        OK = false;
                    }
                }
                if (OK)
                {
                    // switch (rec.ResultItem.GetComponent<Item>().id)
                    // {
                    //     case 0:     
                    //           break;
                    //  }
                    Instantiate(rec.ResultItem, transform.position + Vector3.up, Quaternion.identity);
                    foreach (Transform tr in ItemsOnMe.Slots)
                    {
                        if (tr.childCount == 1)
                        {
                            Destroy(tr.GetChild(0).gameObject);
                        }
                    }
                    break;
                }
            }
        }
        static bool ism1(int i)
        {
            if (i == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

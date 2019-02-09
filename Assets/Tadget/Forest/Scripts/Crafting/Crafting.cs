namespace Tadget
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Crafting : MonoBehaviour
    {
        public Storage itemsOnMe;
        public List<Recipe> recipies;

        public void Check4Craft()
        {
            foreach (Recipe recipe in recipies)
            {
                List<int> itemsOnTable = new List<int>();
                foreach (Transform tr in itemsOnMe.slots)
                {
                    if (tr.childCount == 1)
                    {
                        itemsOnTable.Add(tr.GetChild(0).GetComponent<Item>().id);
                    }
                    else
                    {
                        itemsOnTable.Add(-1);
                    }
                }

                bool OK = true;

                if (recipe.orderSensitive)
                {
                    if (recipe.ingredients.Count == itemsOnTable.Count)
                    {
                        for (int i = 0; i < recipe.ingredients.Count; i++)
                        {
                            if (recipe.ingredients[i] != itemsOnTable[i])
                            {
                                OK = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    itemsOnTable.RemoveAll(x => x == -1);
                    if (recipe.ingredients.Count == itemsOnTable.Count)
                    {
                        foreach (int id in recipe.ingredients)
                        {
                            if (!itemsOnTable.Contains(id))
                            {
                                OK = false;
                                break;
                            }
                            else
                            {
                                itemsOnTable.Remove(id);
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
                    Instantiate(recipe.resultItem, transform.position + Vector3.up, Quaternion.identity);
                    foreach (Transform tr in itemsOnMe.slots)
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
    }
}

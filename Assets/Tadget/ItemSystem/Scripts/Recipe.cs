using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Recipe", menuName = "Crafting recipe")]
public class Recipe : ScriptableObject
{
    [Tooltip("List of ingredients required for crafting specified Item")]
    public List<int> Ingredients;
    [Space]
    [Tooltip("Only craft item if materials placed in right places \nHow ingredients are ordered in list corresponds to crafting device slots order")]
    public bool OrderSensitive = false;
    [Space]
    [Tooltip("Set prefab of item here")]
    public GameObject ResultItem;
}

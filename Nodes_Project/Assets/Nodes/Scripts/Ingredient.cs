using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    public int price;
    public Sprite icon;
    public Color color = Color.white;

    public override bool Equals(object obj)
    {
        return obj is Ingredient ingredient &&
               base.Equals(obj) &&
               name == ingredient.name &&
               hideFlags == ingredient.hideFlags &&
               ingredientName == ingredient.ingredientName &&
               price == ingredient.price &&
               EqualityComparer<Sprite>.Default.Equals(icon, ingredient.icon);
    }
}

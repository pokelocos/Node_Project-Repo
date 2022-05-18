using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IngredientData : ScriptableObject
{
    public string ingredientName; // name (???)
    public int price;
    public Sprite icon;
    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.white;

    public string[] tags = new string[0];

    public override bool Equals(object obj) // whatttt ????
    {
        return obj is IngredientData ingredient &&
               base.Equals(obj) &&
               name == ingredient.name && // con esta comprobacion es mas que suficiente
               hideFlags == ingredient.hideFlags &&
               ingredientName == ingredient.ingredientName && // con esta comprobacion es mas que suficiente
               price == ingredient.price &&
               EqualityComparer<Sprite>.Default.Equals(icon, ingredient.icon);
    }

    public override int GetHashCode()
    {
        int hashCode = 1912113415;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
        hashCode = hashCode * -1521134295 + hideFlags.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ingredientName);
        hashCode = hashCode * -1521134295 + price.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<Sprite>.Default.GetHashCode(icon);
        hashCode = hashCode * -1521134295 + primaryColor.GetHashCode();
        return hashCode;
    }
}

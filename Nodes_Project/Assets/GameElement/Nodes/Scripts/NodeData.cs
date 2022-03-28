using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New node data",menuName = "Node game.../Create node data")]
public class NodeData : ScriptableObject
{
    public Categorie categorie;
    public TechLevel techLevel;
    public Type type;
    public Color color = Color.gray;
    public string description;
    public Sprite icon;
    public float speed = 1f;
    public float successProbability = 1;
    public float productionTime = 5f;
    public int maintainCost = 3;

    public int allowedRecipes = 1;
    public List<NodeActions> onInitialize;
    public List<NodeActions> onWorkFinish;
    public List<IngredientData> defaultInputIngredients;
    public Recipe[] recipes;

    public enum Categorie
    {
        PRODUCTOR, MANUFACTORER, SHOP, LOGISTIC
    }

    public enum Type
    {
        FARM, PLANTATION, FIELD, FACTORY, INDUSTRY, SHOP, MALL
    }

    public enum TechLevel
    {
        T0, T1, T2, T3, T4, T5
    }
}

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
    public List<NodeActions> onInitialize; // esto deberia ser remplazado con lua o cumplir un rol paralelo o no (?)
    public List<NodeActions> onWorkFinish; // esto deberia ser remplazado con lua o cumplir un rol paralelo o no (?)
    public List<IngredientData> defaultInputIngredients; 
    public Recipe[] recipes;

    public enum Categorie // esto deberia ser un string
    {
        PRODUCTOR, MANUFACTORER, SHOP, LOGISTIC
    }

    public enum Type // esto deberia ser un string
    {
        FARM, PLANTATION, FIELD, FACTORY, INDUSTRY, SHOP, MALL
    }

    public enum TechLevel // esto deberia ser un string
    {
        T0, T1, T2, T3, T4, T5
    }
}

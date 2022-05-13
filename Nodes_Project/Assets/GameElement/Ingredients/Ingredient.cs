using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Ingredient // esto deberia ser una mimple estructura dentro de "IngredientData"
{
    [SerializeField] private IngredientData ingredientData;

    [Header("Settings")]
    [SerializeField] private bool compareItem = true;
    [SerializeField] private bool compareItemTags;
    [SerializeField] private bool compareSpecificTags;
    [SerializeField] private string[] specificTags;
    [SerializeField] private bool isOptional;

    public IngredientData IngredientData { get => ingredientData; }
    public bool CompareIngredient { get => compareItem; }
    public bool CompareTags { get => compareItemTags; }
    public bool CompareSpecificTags { get => compareSpecificTags; }
    public bool IsOptional { get => isOptional; }

    public string[] SpecificTags
    {
        get
        {
            return specificTags;
        }
    }

    public static string ListToString(IEnumerable<Ingredient> list) // remove this(?)
    {
        string text = string.Empty;
        foreach (var item in list)
        {
            text += item.ingredientData.ingredientName + " + ";
        }
        return text;
    }
}
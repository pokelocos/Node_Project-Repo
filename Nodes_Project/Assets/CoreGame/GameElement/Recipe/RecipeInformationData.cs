using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RecipeInformationData
{
    public Recipe originalRecipe;

    public struct IngredientStatus
    {
        public IngredientData ingredient;
        public bool status;

        public IngredientStatus(IngredientData ingredient, bool status)
        {
            this.ingredient = ingredient;
            this.status = status;
        }
    }

    public List<IngredientStatus> inputsStatus = new List<IngredientStatus>();

    public bool canCraft;

    public RecipeInformationData(Recipe originalRecipe)
    {
        this.originalRecipe = originalRecipe;

        foreach (var input in originalRecipe.GetIngredients())
        {
            inputsStatus.Add(new IngredientStatus(input.IngredientData, false));
        }
    }
}

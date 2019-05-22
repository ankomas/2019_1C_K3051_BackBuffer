using System.Collections.Generic;
using System.Linq;

namespace TGC.Group.Model.Items.Recipes
{
    public class Recipe
    {
        public IEnumerable<Ingredient> Ingredients{get;}

        public Recipe(IEnumerable<Ingredient> ingredients)
        {
            this.Ingredients = ingredients;
        }

        public bool CanCraft(List<Ingredient> availableIngredients)
        {
            return this.Ingredients.All(ingredient => 
                availableIngredients.Any(ingredient.contains));
        }

        public override string ToString()
        {
            var res = "//";
            this.Ingredients.ToList().ForEach(ingredient =>
            {
                res = res + ingredient.Item.Name + "-" + ingredient.Quantity + "//";
            });
            return res;
        }
    }
}
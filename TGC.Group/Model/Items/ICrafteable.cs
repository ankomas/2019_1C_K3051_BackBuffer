using TGC.Group.Model.Items.Recipes;

namespace TGC.Group.Model.Items
{
    public abstract class ICrafteable:IItem
    {
        public Recipe Recipe;

        public ICrafteable(Recipe recipe)
        {
            this.Recipe = recipe;
        }
    }
}
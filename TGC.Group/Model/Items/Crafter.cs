using System.Collections.Generic;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Items.Materials;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Player;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Items
{
    public class Crafter
    {
        public ICrafteable CraftedItem;

        public static List<ICrafteable> Crafteables { get; } = new List<ICrafteable>
        {
            new OxygenTank(), new GreenPotion(), new InfinityGauntlet()
        };

        public List<ICrafteable> CrafteablesBy(List<Ingredient> ingredients)
        {
            return Crafteables.FindAll(crafteable => crafteable.Recipe.CanCraft(ingredients));
        }

        public void Craft(ICrafteable crafteable, Character character)
        {
            if (!crafteable.Recipe.CanCraft(character.Inventory.AsIngredients())) return;
            
            character.RemoveIngredients(crafteable.Recipe.Ingredients);

            SoundManager.Play(SoundManager.Craft);
            
            this.CraftedItem = crafteable;
        }
    }
}
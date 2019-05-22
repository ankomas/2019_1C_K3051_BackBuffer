using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items.Equipment
{
    public abstract class IEquipable : IItem, ICrafteable
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ItemType type { get; }
        public abstract CustomSprite Icon { get; }
        public abstract TGCVector2 DefaultScale { get; }
        public void Use(Character character)
        {
            character.Equip(this);
        }

        public abstract void ApplyEffect(Stats character);
        public Recipe CrafteableRecipe { get; }
    }
}
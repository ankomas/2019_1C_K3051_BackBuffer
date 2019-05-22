using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items.Equipment
{
    public abstract class IEquipable : ICrafteable
    {
        protected IEquipable(Recipe Recipe) : base(Recipe)
        {
            this.Recipe = Recipe;
        }
        public override void Use(Character character)
        {
            character.Equip(this);
        }

        public abstract void ApplyEffect(Stats character);
    }
}
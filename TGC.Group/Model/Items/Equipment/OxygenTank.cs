using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items.Equipment
{
    public class OxygenTank : IEquipable
    {
        private const int Capacity = 10;
        public static readonly Recipe Recipe = 
            new Recipe(new List<Ingredient>{new Ingredient(new Gold(), 3)});

        public override string Name { get; } = "Oxygen tank";
        public override string Description { get; } = "Increments the oxygen capacity in " + Capacity;
        public override ItemType type { get; } = ItemType.EQUIPABLE;
        public Recipe CrafteableRecipe { get; } = Recipe;

        public override CustomSprite Icon { get; } = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Fish);
        public override TGCVector2 DefaultScale { get; } = new TGCVector2(1f, 1f);

        public override void ApplyEffect(Stats character)
        {
            character.Oxygen += Capacity;
        }
    }
}
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Consumables;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items.Materials
{
    public class GreenPotion : ICrafteable
    {
        public static readonly Recipe _Recipe = 
            new Recipe(new List<Ingredient>{new Ingredient(new GreenCoral(), 5)});

        public new Recipe Recipe => _Recipe;
        public override string Name { get; } = "Green potion";
        public override string Description { get; } = "Used for purifying metals";

        public override ItemType type { get; } = ItemType.MATERIAL;
        public override CustomSprite Icon { get; }
        public override TGCVector2 DefaultScale { get; } = new TGCVector2(.15f, .15f);
        public Stats stats { get; } = new Stats(0, 100);

        public override void Use(Character character)
        {
            //TODO REFACTOR THIS
            character.UpdateStats(stats);
            character.RemoveItem(this);
        }

        public GreenPotion() : base(_Recipe)
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.GreenPotion);
            icon.Scaling = DefaultScale;
            Icon = icon;
        }
    }
}
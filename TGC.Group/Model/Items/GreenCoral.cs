using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class GreenCoral : IItem
    {
        public override string Name { get; } = "Green coral";
        public override string Description { get; } = "Used for potion crafting";
        public override ItemType type { get; } = ItemType.MATERIAL;
        public override CustomSprite Icon { get; }
        public override TGCVector2 DefaultScale { get; } = new TGCVector2(.05f, .05f);
        public override void Use(Character character)
        {
            //TODO something?
        }

        public GreenCoral()
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Plant);
            icon.Scaling = DefaultScale;
            Icon = icon;
        }
    }
}
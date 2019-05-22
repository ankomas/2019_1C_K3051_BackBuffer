using TGC.Core.Mathematica;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class Gold : IItem
    {
        public override string Name { get; } = "Gold";
        public override string Description { get; } = "High quality metal";
        public override ItemType type { get; } = ItemType.MATERIAL;
        public override CustomSprite Icon { get; }
        public sealed override TGCVector2 DefaultScale { get; } = new TGCVector2(.042f, .042f);
        public override void Use(Character character)
        {
            //TODO something?
        }

        public Gold()
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Gold);
            icon.Scaling = DefaultScale;
            Icon = icon;
        }
    }
}
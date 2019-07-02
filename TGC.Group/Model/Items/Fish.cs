using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items.Consumables;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class Fish : IConsumable
    {
        public override string Name { get; } = "Fish";
        public override string Description { get; } = "Looks delicious!";
        public override ItemType type { get; } = ItemType.CONSUMABLE;
        public override CustomSprite Icon { get; }
        static List<TgcMesh> _meshes = MeshesForShip.O2Mesh;
        public override List<TgcMesh> Meshes => _meshes;
        public sealed override TGCVector2 DefaultScale { get; } = new TGCVector2(.1f, .05f);

        public override Stats stats => new Stats(0, 10);

        public Fish()
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.Fish);
            icon.Scaling = DefaultScale;
            Icon = icon;
        }

        public override void Use(Character character)
        {
            SoundManager.Play(SoundManager.Eat);
            base.Use(character);
        }
    }
}
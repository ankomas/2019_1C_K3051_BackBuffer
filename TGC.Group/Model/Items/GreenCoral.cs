using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class GreenCoral : IItem
    {
        public override string Name { get; } = "Coral";
        public override string Description { get; } = "Used for potion crafting";
        public override ItemType type { get; } = ItemType.MATERIAL;
        public override CustomSprite Icon { get; }
        static List<TgcMesh> _meshes = MeshesForShip.O2Mesh;
        public override List<TgcMesh> Meshes => _meshes;
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
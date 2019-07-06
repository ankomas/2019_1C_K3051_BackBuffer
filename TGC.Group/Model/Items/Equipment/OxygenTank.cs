using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Utils;
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
        public override CustomSprite Icon { get; } = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.OxygenTank);
        public override List<TgcMesh> Meshes => MeshesForShip.O2Mesh;
        public override TGCVector2 DefaultScale { get; } = new TGCVector2(.075f, .075f);
        public OxygenTank() : base(Recipe)
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.OxygenTank);
            icon.Scaling = DefaultScale;
            Icon = icon;
        }

        public override void Use(Character character)
        {
            SoundManager.Play(SoundManager.Oxygen);
            base.Use(character);
        }

        public override void ApplyEffect(Stats character)
        {
            character.Oxygen += Capacity;
        }
    }
}
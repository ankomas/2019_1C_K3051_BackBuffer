using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items.Equipment;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Items.Type;
using TGC.Group.Model.Player;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class InfinityGauntlet: IEquipable
    {
        public static readonly Recipe Recipe = 
            new Recipe(new List<Ingredient>{new Ingredient(new Gold(), 3)});

        private TgcMesh Mesh;
        
        public InfinityGauntlet() : base(Recipe)
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.InfinityGauntlet);
            icon.Scaling = DefaultScale;
            Icon = icon;
            
            Mesh = new TgcSceneLoader()
                .loadSceneFromFile(Game.Default.MediaDirectory + "infinity-gauntlet-TgcScene.xml").Meshes[0];
            Mesh.Scale = new TGCVector3(.035f,.035f,.035f);
            
        }

        public override string Name { get; } = "Infinity Gauntlet";
        public override string Description { get; } = "GG";
        public override ItemType type { get; } = ItemType.EQUIPABLE;
        public override CustomSprite Icon { get; } = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.InfinityGauntlet);
        public override TGCVector2 DefaultScale { get; } = new TGCVector2(.1f, .1f);

        public void Update(Camera camera)
        {
            Mesh.Position = camera.LookAt + TGCVector3.TransformNormal(new TGCVector3(-10,-4, -9f), camera.cameraRotation);
            Mesh.Rotation = new TGCVector3(
                FastMath.QUARTER_PI * 0.05f + FastMath.Clamp(camera.updownRot, -FastMath.QUARTER_PI * 0.2f, FastMath.QUARTER_PI * 0.2f), 
                camera.leftrightRot + FastMath.QUARTER_PI * 0.9f, 
                -FastMath.QUARTER_PI * 1.4f + FastMath.Clamp(camera.updownRot , -FastMath.QUARTER_PI * 0.2f, FastMath.QUARTER_PI * 0.2f)
                );
        }

        public void Render()
        {
            Mesh.Render();       
        }
        
        public override void ApplyEffect(Stats character)
        {
            throw new System.NotImplementedException();
        }
    }
}
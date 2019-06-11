using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Input;
using TGC.Group.Model.Items.Recipes;
using TGC.Group.Model.Resources.Meshes;
using TGC.Group.Model.Resources.Sprites;
using TGC.Group.Model.Scenes;
using TGC.Group.TGCUtils;

namespace TGC.Group.Model.Items
{
    public class InfinityGauntlet: Weapon
    {
        public static readonly Recipe Recipe = new Recipe(new List<Ingredient>
        {
            new Ingredient(new Gold(), 1)
        });
        
        public static Effect death = 
            TGCShaders.Instance.LoadEffect(Game.Default.ShadersDirectory + "NoMeQuieroIrSrStark.fx");

        
        private Random random = new Random();

        private bool inAttack;
        private float elapsedTimeSinceAttack;
        private IEnumerable<Element> elementsToAttack = new List<Element>();
        private float elapsedTime = 0f;
        
        
        private static TgcMesh CreateMesh()
        {
            return new TgcSceneLoader()
                .loadSceneFromFile(Game.Default.MediaDirectory + "infinity-gauntlet-TgcScene.xml").Meshes[0];
        }

        public InfinityGauntlet() : base(Recipe, CreateMesh())
        {
            CustomSprite icon = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.InfinityGauntlet);
            icon.Scaling = DefaultScale;
            Icon = icon;
            Mesh.Scale = new TGCVector3(.035f,.035f,.035f);

        }

        public override string Name { get; } = "Infinity Gauntlet";
        public override string Description { get; } = "GG";
        public override CustomSprite Icon { get; } = BitmapRepository.CreateSpriteFromBitmap(BitmapRepository.InfinityGauntlet);
        public override List<TgcMesh> Meshes => MeshesForShip.InfinityGauntlet;
        public override TGCVector2 DefaultScale { get; } = new TGCVector2(.1f, .1f);

        public override void Update(Camera camera)
        {
            Mesh.Position = camera.LookAt + TGCVector3.TransformNormal(new TGCVector3(-10,-4, -9f), camera.cameraRotation);
            Mesh.Rotation = new TGCVector3(
                FastMath.QUARTER_PI * 0.05f + FastMath.Clamp(camera.updownRot, -FastMath.QUARTER_PI * 0.2f, FastMath.QUARTER_PI * 0.2f), 
                camera.leftrightRot + FastMath.QUARTER_PI * 0.9f, 
                -FastMath.QUARTER_PI * 1.4f + FastMath.Clamp(camera.updownRot , -FastMath.QUARTER_PI * 0.2f, FastMath.QUARTER_PI * 0.2f)
            );
        }
        public override void Attack(World world, TgcD3dInput input) 
        {
            if (inAttack)
            {
                elapsedTimeSinceAttack += GameModel.GlobalElapsedTime;
                foreach (var element in elementsToAttack)
                {

                    element.Mesh.Effect.SetValue("elapsedTime", elapsedTimeSinceAttack * 0.5f);
                }
                
                
                if (elapsedTimeSinceAttack > 5)
                {
                    foreach (var element in elementsToAttack)
                    {
                        world.Remove(element);
                    }

                    elementsToAttack = new List<Element>();
                    inAttack = false;
                    elapsedTimeSinceAttack = 0;
                }

            }
            
            if (GameInput._Attack.IsPressed(input))
            {
                
                Mesh.Position = new TGCVector3(
                    Mesh.Position.X + (float)random.NextDouble() / 8,
                    Mesh.Position.Y + (float)random.NextDouble() / 8, 
                    Mesh.Position.Z + (float)random.NextDouble() / 8
                    );
                
                if (!inAttack)
                { 
                   inAttack = true;
                   elapsedTimeSinceAttack += GameModel.GlobalElapsedTime;
                   elementsToAttack = world.elementsToUpdate
                       .Take(world.elementsToUpdate.Count / 1)
                       .Where(element => element.Mesh != null);
                   foreach (var element in elementsToAttack)
                   {
                           element.Mesh.Technique = "RenderScene";
                           element.Mesh.Effect = death;
                           element.Mesh.Effect.SetValue("elapsedTime", elapsedTimeSinceAttack);

                   }
                   
                }
            }
        }
               

    }
}
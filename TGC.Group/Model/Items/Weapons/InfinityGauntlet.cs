using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
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
        
        private static Random random = new Random();

        private float transcurredTime = 0f;


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
            if (GameInput._Attack.IsDown(input))
            {
                transcurredTime += GameModel.GlobalElapsedTime;
                
                Mesh.Position = new TGCVector3(
                    Mesh.Position.X + (float)random.NextDouble() / 8,
                    Mesh.Position.Y + (float)random.NextDouble() / 8, 
                    Mesh.Position.Z + (float)random.NextDouble() / 8
                    );

                if (transcurredTime > 2)
                {
                 
                    foreach (var element in world.elementsToUpdate.Take(world.elementsToUpdate.Count / 2 ))
                    { 
                        world.Remove(element);
                    }

                    transcurredTime = 0;
                }
            }
            else
            {
                transcurredTime = 0;
            }
        }

    }
}
﻿using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Group.Model.Items;
using TGC.Group.Model.Movements;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Elements
{
    class Fish : Element
    {

        private float ProximityRadius = FastMath.Pow2(700f);
        private bool runningAway;
        private RandomMovement RandomMov { get; set; }
        private EscapeFromPosition EscapeMov { get; set; }
        
        public static Effect movement = TGCShaders.Instance.LoadEffect(Game.Default.ShadersDirectory + "FishMovement.fx");
        public static Effect FishEffect = ShaderRepository.NoMeQuieroIrSrStark;
        public Fish(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
            EscapeMov = new EscapeFromPosition(new TGCVector3(1f, 0f, 0f), 0.1f, 30f );
            RandomMov = new RandomMovement(new TGCVector3(1f, 0f, 0f), 0.3f, 10f);
            //Mesh.Technique = "RenderScene";
            //Mesh.Effect = movement;
            Mesh.Effect = FishEffect;
            Mesh.Technique = "Fish";
        }

        public override void Update(Camera camera)
        {
            var movementToApply = Move(camera);
            PhysicsBody.CenterOfMassTransform *= movementToApply.Translation.ToBsMatrix;
            Mesh.Position = new TGCVector3(PhysicsBody.CenterOfMassPosition);
            Mesh.Rotation += movementToApply.AnglesToRotate;
            base.Update(camera);
        }

        public override void Render()
        { 
            //this.Mesh.Effect.SetValue("time", GameModel.GlobalTime);
            base.Render();
        }

        private MovementToApply Move(Camera camera)
        {
            return IsInTheRange(camera) ? EscapeFromCamera(camera) : MoveRandomly();
        }

        private MovementToApply EscapeFromCamera(Camera camera)
        {
            if (!runningAway)
            {
                //SoundManager.Play(SoundManager.Bubbles);
                EscapeMov.LookAt = RandomMov.LookAt;
                runningAway = true;
            }
            return EscapeMov.Move(Mesh.Position, camera.Position);
        }

        private MovementToApply MoveRandomly()
        {
            if (runningAway)
            {
                RandomMov.LookAt = EscapeMov.LookAt;
                runningAway = false;
            }

            return RandomMov.Move(Mesh.Position, TGCVector3.Empty);
        }
        private bool IsInTheRange(Camera camera)
        {
            return (Mesh.Position - camera.Position).LengthSq() <= ProximityRadius;
        }


        public override IItem item { get; } = new Items.Fish();

        public override bool HasDefaultShader()
        {
            return true;
        }
    }
}

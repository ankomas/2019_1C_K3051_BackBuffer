using System.Drawing;
using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Group.Form;
using TGC.Group.Model.Items;
using TGC.Group.Model.Movements;

namespace TGC.Group.Model.Elements
{
    class Fish : Element
    {

        private float ProximityRadius = FastMath.Pow2(700f);
        private bool runningAway;
        private RandomMovement RandomMov { get; set; }
        private EscapeFromPosition EscapeMov { get; set; }
        
        public static Effect movement = TGCShaders.Instance.LoadEffect(Game.Default.ShadersDirectory + "FishMovement.fx");
        
        public Fish(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
            EscapeMov = new EscapeFromPosition(new TGCVector3(1f, 0f, 0f), 0.1f, 60f );
            RandomMov = new RandomMovement(new TGCVector3(1f, 0f, 0f), 0.3f, 20f);
            model.Effect = movement;
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
            this.Mesh.Effect.SetValue("time", GameModel.GlobalElapsedTime);
            this.Mesh.Render();
        }

        private MovementToApply Move(Camera camera)
        {
            return IsInTheRange(camera) ? EscapeFromCamera(camera) : MoveRandomly();
        }

        private MovementToApply EscapeFromCamera(Camera camera)
        {
            if (!runningAway)
            {
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

    }
}

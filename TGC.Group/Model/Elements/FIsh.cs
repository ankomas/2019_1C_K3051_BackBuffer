using BulletSharp;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;
using TGC.Group.Model.Movements;

namespace TGC.Group.Model.Elements
{
    class Fish : Element
    {

        private float ProximityRadius = FastMath.Pow2(50f);
        private RandomMovement RandomMov { get; set; }
        private EscapeFromPosition EscapeMov { get; set; }
        public Fish(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
            EscapeMov = new EscapeFromPosition(new TGCVector3(1f, 0f, 0f), 0.1f, 100f );
            RandomMov = new RandomMovement(new TGCVector3(1f, 0f, 0f), 0.1f, 20f);
        }

        public override void Update(Camera camera)
        {
            var movementToApply = Move(camera);
            PhysicsBody.CenterOfMassTransform *= movementToApply.Translation.ToBsMatrix;
            Mesh.Position = new TGCVector3(PhysicsBody.CenterOfMassPosition);
            Mesh.Rotation += movementToApply.AnglesToRotate;
            base.Update(camera);

        }

        private MovementToApply Move(Camera camera)
        {
            return IsInTheRange(camera) ? EscapeFromCamera(camera) : MoveRandomly();
        }

        private MovementToApply EscapeFromCamera(Camera camera)
        {
            return EscapeMov.Move(Mesh.Position, camera.Position);
        }

        private MovementToApply MoveRandomly()
        {
            return RandomMov.Move(Mesh.Position, TGCVector3.Empty);
        }

        private bool IsInTheRange(Camera camera)
        {
            return (Mesh.Position - camera.Position).LengthSq() <= ProximityRadius;
        }


        public override IItem item { get; } = new Items.Fish();

    }
}

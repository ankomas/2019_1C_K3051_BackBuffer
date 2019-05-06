using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements.ElementFactories
{
    internal class Shark : Element
    {
        public Shark(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }

        public override IRenderObject getCollisionVolume()
        {
            CapsuleShapeX capsule = (CapsuleShapeX)PhysicsBody.CollisionShape;

            var radius = new TGCVector3(capsule.Radius + capsule.HalfHeight, capsule.Radius, capsule.Radius);

            return new TgcBoundingElipsoid(new TGCVector3(PhysicsBody.CenterOfMassPosition), radius);
        }
    }
}
using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using BulletSharp;
using BulletSharp.Math;

namespace TGC.Group.Model
{
    public class Element : Collisionable
    {
        public TgcMesh Mesh { get; }
        public RigidBody PhysicsBody { get; set; }


        public Element(TgcMesh model, RigidBody rigidBody)
        {
            this.Mesh = model;
            this.PhysicsBody = rigidBody;
        }

        public void Update()
        {
            Mesh.Position = new TGCVector3(PhysicsBody.CenterOfMassPosition.X, PhysicsBody.CenterOfMassPosition.Y, PhysicsBody.CenterOfMassPosition.Z);
            Mesh.Transform = 
                TGCMatrix.Scaling(Mesh.Scale) *
                new TGCMatrix(PhysicsBody.CenterOfMassTransform);

            return;
        }

        public void Render()
        {
            Mesh.Render();
            return;
        }

        public void Dispose()
        {
            Mesh.Dispose();
            PhysicsBody.Dispose();
            return;
        }

        public override IRenderObject getCollisionVolume()
        {
            Vector3 aabbMin, aabbMax;
            PhysicsBody.GetAabb(out aabbMin, out aabbMax);
            return new TgcBoundingAxisAlignBox(new TGCVector3(aabbMin), new TGCVector3(aabbMax));
        }
    }
}
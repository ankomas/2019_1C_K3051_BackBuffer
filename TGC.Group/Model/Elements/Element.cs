using TGC.Core.BoundingVolumes;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.Text;
using System.Drawing;

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

        public virtual void Update(Camera camera)
        {
            Mesh.Position = new TGCVector3(PhysicsBody.CenterOfMassPosition);
            Mesh.Transform = 
                TGCMatrix.Scaling(Mesh.Scale) *
                new TGCMatrix(PhysicsBody.CenterOfMassTransform);
        }

        public virtual void Render()
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
using System.Runtime.CompilerServices;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model.Elements
{
    public class Ship : Element
    {
        private TgcScene ship;
        
        public Ship(TgcScene ship, RigidBody rigidBody) : base(null, rigidBody)
        {
            this.ship = ship;
        }
        
        public override void Update(Camera camera)
        {
            this.ship.Meshes.ForEach(mesh =>
            {
                mesh.Position = new TGCVector3(this.PhysicsBody.CenterOfMassPosition.X,
                        this.PhysicsBody.CenterOfMassPosition.Y, this.PhysicsBody.CenterOfMassPosition.Z);
                mesh.Transform = TGCMatrix.Scaling(mesh.Scale) *
                    new TGCMatrix(this.PhysicsBody.CenterOfMassTransform);
            });
            
            this.Selectable = false;
        }

        public override TGCVector3 getPosition()
        {
            return this.ship.Meshes[0].Position;
        }

        public override void Render()
        {
            this.ship.RenderAll();
            if (this.Selectable)
            {
                getCollisionVolume().Render();
            }
        }

        public override void Dispose(AquaticPhysics physics)
        {
            base.Dispose(physics);
            this.ship?.DisposeAll();
        }
        
        public override IRenderObject getCollisionVolume() 
        { 
            Vector3 aabbMin, aabbMax;
            PhysicsBody.GetAabb(out aabbMin, out aabbMax);
            return new TgcBoundingAxisAlignBox(new TGCVector3(aabbMin), new TGCVector3(aabbMax));
        }

        public override IItem item { get; } = null;
    }
}
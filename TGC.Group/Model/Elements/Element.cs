using System;
using System.Linq;
using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;
using TGC.Group.Model.UI;
using TGC.Group.Model.Utils;
using Chunk = TGC.Group.Model.Chunks.Chunk;

namespace TGC.Group.Model.Elements
{
    public abstract class Element: Collisionable
    {

        public TgcMesh Mesh { get; }
        public RigidBody PhysicsBody { get; set; }
        public bool Selectable { get; set; }

        public abstract IItem item { get; }
/*
        private Effect effect;
        public Effect Effect
        {
            set
            {
                Mesh.Effect = effect = value;
                Mesh.Technique = "FedeTechnique";
            }
            get
            {
                return effect;
            }
        }
*/
        public Element(TgcMesh model, RigidBody rigidBody)
        {
            Mesh = model;
            PhysicsBody = rigidBody;
        }
        public bool isIntersectedBy(TgcRay ray)
        {
            return asCube().isIntersectedBy(ray);
        }

        public virtual void Update(Camera camera)
        {
            UpdateMesh();
            Selectable = false;
        }

        private void UpdateMesh()
        {
            Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale) * 
                             TGCMatrix.RotationYawPitchRoll(Mesh.Rotation.Y, Mesh.Rotation.X, Mesh.Rotation.Z) *
                             new TGCMatrix(PhysicsBody.CenterOfMassTransform);
        }

        public virtual void Render()
        {
            Mesh.Render();
            
            if(Selectable)
                getCollisionVolume().Render();
        }

        public virtual void Dispose(AquaticPhysics physics)
        {
            Mesh?.Dispose();
            
            if (PhysicsBody != null)
            {
                physics.Remove(PhysicsBody);
                PhysicsBody.Dispose();
            }
        }

        public virtual TGCVector3 getPosition()
        {
            return Mesh.Position;
        }

        public override IRenderObject getCollisionVolume() 
        { 
            Vector3 aabbMin, aabbMax;
            PhysicsBody.GetAabb(out aabbMin, out aabbMax);
            return new TgcBoundingAxisAlignBox(new TGCVector3(aabbMin), new TGCVector3(aabbMax));
        }

        public Cube asCube()
        {
            var aabb = (TgcBoundingAxisAlignBox) getCollisionVolume();
            var cube = new Cube(aabb.PMin, aabb.PMax);
            return cube;
        }

        public void yPosition(MySimpleTerrain floor)
        {
            var x = this.Mesh.Position.X % Chunk.DefaultSize.X;
            var z = this.Mesh.Position.Z % Chunk.DefaultSize.Z;

            var xLength = floor.HeightmapData.GetLength(1);
            var zLength = floor.HeightmapData.GetLength(0);
            
            var xScale = xLength / Chunk.DefaultSize.X;
            var zScale = zLength / Chunk.DefaultSize.Z;
            
            var xIndex = x < 0 ? xLength - (int) Math.Abs(x * xScale) : (int) Math.Abs(x * xScale);
            var zIndex = z > 0 ? zLength - (int) Math.Abs(z * zScale) : (int) Math.Abs(z * zScale);
            
            var y = floor.scaled(xIndex, zIndex);

            var cube = asCube();
            var despl = ((cube.PMax - cube.PMin).X + (cube.PMax - cube.PMin).Z) / 2;
            
            this.PhysicsBody.Translate(new Vector3(-despl, y, -despl));
        }
    }
}
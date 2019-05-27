﻿using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Player;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    public abstract class Entity : Collisionable
    {
        public TgcMesh Mesh { get; }
        public RigidBody RigidBody { get; }

        public Entity(TgcMesh mesh, RigidBody rigid)
        {
            Mesh = mesh;
            RigidBody = rigid;
        }


        public virtual void Render()
        {
            Mesh.Render();

        }
        public virtual void Update(Camera camera, Character character)
        {
            Mesh.Position = new TGCVector3(RigidBody.CenterOfMassPosition);
            Mesh.Transform =
                TGCMatrix.Scaling(Mesh.Scale) *
                new TGCMatrix(RigidBody.CenterOfMassTransform);
        }
        public abstract void Dispose();
        public override abstract IRenderObject getCollisionVolume();
    }
}
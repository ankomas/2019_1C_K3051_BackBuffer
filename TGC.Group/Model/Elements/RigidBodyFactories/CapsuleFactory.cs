﻿using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.Math;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Elements.RigidBodyFactories
{
    class CapsuleFactory : IRigidBodyFactory
    {
        public RigidBody Create(TgcMesh mesh)
        {
            var mass = 10f;
            var radius = mesh.BoundingBox.calculateAxisRadius();

            CapsuleShape capsule;
            if (radius.X >= radius.Y)
            {
                capsule = new CapsuleShapeX(radius.Y, radius.X - radius.Y);
            }
            else
            {
                capsule = new CapsuleShape(radius.X, radius.Y - radius.X);
            }
            RigidBody rigidBody = CreateRigidBody(mesh.Position, mass, capsule);

            return rigidBody;
        }
        
        private static RigidBody CreateRigidBody(TGCVector3 position, float mass, CapsuleShape capsule)
        {
            var inertia = capsule.CalculateLocalInertia(mass);
            var transform = TGCMatrix.Translation(position);

            var motionState = new DefaultMotionState(transform.ToBsMatrix);
            var rigidBodyInfo = new RigidBodyConstructionInfo(mass, motionState, capsule, inertia);
            return new RigidBody(rigidBodyInfo)
            {
                AngularFactor = Vector3.UnitY
            };

        }

        public RigidBody Create(TGCVector3 position, float radius, float height)
        {
            var mass = 60f;
            var capsule = new CapsuleShape(radius, height);
            return CreateRigidBody(position, mass, capsule);
        }

        public RigidBody CreateShark(TgcMesh mesh)
        {
            var mass = 1000f;
            var radius = mesh.BoundingBox.calculateAxisRadius();
;
            return CreateRigidBody(
                mesh.Position, 
                mass, 
                new CapsuleShapeX(radius.Y, radius.X * 1.5f - radius.Y)
            );

        }
    }
}

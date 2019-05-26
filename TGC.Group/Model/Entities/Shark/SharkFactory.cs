using System;
using System.Dynamic;
using BulletSharp.Math;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Entities;
using TGC.Group.Model.Resources.Meshes;

namespace TGC.Group.Model.Elements
{
    public class SharkFactory
    {
        private const float DefaultRotVelocity = 0.05f;
        private const float DefaultTranslationVelocity = 10f;

        public static Shark Create( 
            TGCVector3 position, 
            float rotationVelocity = DefaultRotVelocity , 
            float translationVelocity = DefaultTranslationVelocity
            )
        {
            var mesh = SharkMesh.Get();
            mesh.Position = position; //new TGCVector3(30, 0, -2000);
            mesh.UpdateMeshTransform();
            
            var rigidBody = new CapsuleFactory().CreateShark(mesh); 
            AquaticPhysics.Instance.Add(rigidBody);

            return new Shark(mesh, rigidBody)
            {
                MovementToCamera = new MovementToPosition(Vector3.UnitX, rotationVelocity, translationVelocity)
            };
        }

    }
}
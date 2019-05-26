using System;
using System.Drawing;
using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Entities;
using TGC.Group.Model.Player;
using Matrix = Microsoft.DirectX.Matrix;


namespace TGC.Group.Model.Elements
{
    public class Shark : Entity
    {
        private static readonly TgcText2D DrawText = new TgcText2D();
        public MovementToPosition MovementToCamera { get; set; }
        private bool dead;

        public Shark(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody) { }

        public override void Update(Camera camera, Character character)
        {
            var difference = camera.Position.ToBulletVector3() - RigidBody.CenterOfMassPosition;

            var sharkBody = (CapsuleShapeX)RigidBody.CollisionShape;
            var cameraBody = (CapsuleShape)camera.RigidBody.CollisionShape;

            if (VerifyCollision(difference, sharkBody, cameraBody))
            {
                character.Hit(10);
            }

            difference.Normalize();
           
            MovementToCamera.Move(Mesh, RigidBody, camera.Position);
            
            base.Update(camera, character);
        }

        private bool VerifyCollision(Vector3 difference, CapsuleShapeX sharkBody, CapsuleShape cameraBody)
        {
            var epsilon = 10f;
            return FastMath.Pow2(difference.X) <=
                FastMath.Pow2(sharkBody.Radius + sharkBody.HalfHeight - cameraBody.Radius) * epsilon &&
                FastMath.Pow2(difference.Y) <=
                FastMath.Pow2(sharkBody.Radius - (cameraBody.Radius + cameraBody.HalfHeight)) * epsilon&&
                FastMath.Pow2(difference.Z) <=
                FastMath.Pow2(sharkBody.Radius - cameraBody.Radius) * epsilon ;
        }

        public override void Render()
        {
            base.Render();
            if (dead)
            {
                var point = GetCenter();
                DrawText.drawText("Hit", point.X, point.Y, Color.Red);
            }

        }
        private static Point GetCenter()
        {
            return new Point(
                D3DDevice.Instance.Device.Viewport.Width / 2,
                D3DDevice.Instance.Device.Viewport.Height / 2
                );
        }
        
        public override IRenderObject getCollisionVolume()
        {
            CapsuleShapeX capsule = (CapsuleShapeX)RigidBody.CollisionShape;

            var radius = new TGCVector3(capsule.Radius + capsule.HalfHeight, capsule.Radius, capsule.Radius);

            return new TgcBoundingElipsoid(new TGCVector3(RigidBody.CenterOfMassPosition), radius);
        }

        public override void Dispose()
        {
            Mesh.Dispose();
            RigidBody.Dispose();
        }
    }
}
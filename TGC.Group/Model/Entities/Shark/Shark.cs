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
using TGC.Group.Model.Movements;
using TGC.Group.Model.Player;
using Matrix = Microsoft.DirectX.Matrix;


namespace TGC.Group.Model.Elements
{
    public class Shark : Entity
    {
        private static readonly TgcText2D DrawText = new TgcText2D();

        public Shark(TgcMesh model, RigidBody rigidBody, Movement movement) : base(model, rigidBody, movement) { }

        public override void Update(Camera camera, Character character)
        {
            TryToAttack(camera, character);

            base.Update(camera, character);
        }

        private void TryToAttack(Camera camera, Character character)
        {
            var difference = camera.Position.ToBulletVector3() - RigidBody.CenterOfMassPosition;

            var sharkBody = (CapsuleShapeX) RigidBody.CollisionShape;
            var cameraBody = (CapsuleShape) camera.RigidBody.CollisionShape;

            if (VerifyCollision(difference, sharkBody, cameraBody))
            {
                character.Hit(10);
            }
        }


        private bool VerifyCollision(Vector3 difference, CapsuleShapeX sharkBody, CapsuleShape cameraBody)
        {
            var epsilon = 30f;
            return FastMath.Pow2(difference.X) <=
                FastMath.Pow2(sharkBody.Radius + sharkBody.HalfHeight - cameraBody.Radius) * epsilon &&
                FastMath.Pow2(difference.Y) <=
                FastMath.Pow2(sharkBody.Radius - (cameraBody.Radius + cameraBody.HalfHeight)) * epsilon&&
                FastMath.Pow2(difference.Z) <=
                FastMath.Pow2(sharkBody.Radius - cameraBody.Radius) * epsilon ;
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
            Mesh?.Dispose();
            //TODO hack
            if (RigidBody != null)
            {
                AquaticPhysics.Instance.Remove(RigidBody);
                RigidBody.Dispose();
            }
        }
    }
}
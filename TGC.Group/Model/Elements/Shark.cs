using BulletSharp;
using BulletSharp.Math;
using System;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;

namespace TGC.Group.Model.Elements.ElementFactories
{
    internal class Shark : Element
    {
        private static float DefaultVelocity = 5f;
        private static TgcText2D DrawText = new TgcText2D();
        private bool dead = false;

        public Shark(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }

        public override void Update(Camera camera)
        {
            var vector = camera.Position.ToBulletVector3() - PhysicsBody.CenterOfMassPosition;
            vector.Normalize();
            PhysicsBody.Translate(vector * DefaultVelocity);

            var sharkBody = (CapsuleShapeX)PhysicsBody.CollisionShape;
            var cameraBody = (CapsuleShape)camera.RigidBody.CollisionShape;

            var difference = PhysicsBody.CenterOfMassPosition - camera.RigidBody.CenterOfMassPosition;

            if(
            FastMath.Pow2(difference.X) <= FastMath.Pow2(sharkBody.Radius + sharkBody.HalfHeight - cameraBody.Radius) &&
            FastMath.Pow2(difference.Y) <= FastMath.Pow2(sharkBody.Radius - (cameraBody.Radius + cameraBody.HalfHeight)) &&
            FastMath.Pow2(difference.Z) <= FastMath.Pow2(sharkBody.Radius - cameraBody.Radius)
             )
            {
                dead = true;

            }

            base.Update(camera);

        }

        public override void Render()
        {
            base.Render();
            if (dead)
            {
                var point = GetCenter();
                DrawText.drawText("TE MORISTE WEEE xddxdxd", point.X, point.Y, Color.Red);
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
            CapsuleShapeX capsule = (CapsuleShapeX)PhysicsBody.CollisionShape;

            var radius = new TGCVector3(capsule.Radius + capsule.HalfHeight, capsule.Radius, capsule.Radius);

            return new TgcBoundingElipsoid(new TGCVector3(PhysicsBody.CenterOfMassPosition), radius);
        }
    }
}
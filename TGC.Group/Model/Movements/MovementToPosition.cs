using System;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Entities
{
    public class MovementToPosition
    {
        private Vector3 LookAt;
        private float RotationVelocity;
        private float TranslationVelocity;
        
        public MovementToPosition(Vector3 lookAt, float rotationVelocity, float translationVelocity)
        {
            LookAt = lookAt;
            RotationVelocity = rotationVelocity;
            TranslationVelocity = translationVelocity;
        }

        
        
        public void Move(TgcMesh mesh, RigidBody rigidBody, TGCVector3 destination)
        {
            var angles = AnglesToRotate(mesh.Position, destination);
            UpdateLookAtVector(angles);
            mesh.Rotation += new TGCVector3(angles);
            rigidBody.CenterOfMassTransform *= Matrix.Translation(-LookAt * TranslationVelocity);
        }

        private Vector3 AnglesToRotate(TGCVector3 position, TGCVector3 destination)
        {
            var directions =  DirectionToRotate(position.ToBulletVector3(), destination.ToBulletVector3());
            directions.Normalize();
            return LimitAngles(directions * RotationVelocity);
            
        }

        private Vector3 LimitAngles(Vector3 anglesToRotate)
        {
            return new Vector3(0, anglesToRotate.Y, 0);
        }

        private void UpdateLookAtVector(Vector3 angles)
        {
            
            LookAt = Vector3.TransformNormal(LookAt, 
                Matrix.RotationYawPitchRoll(angles.Y,angles.X,angles.Z)
                );
        }
        

        private Vector3 DirectionToRotate(Vector3 myPosition, Vector3 destination)
        { 
            return Vector3.Cross( destination - myPosition, LookAt);
        }

    }
}
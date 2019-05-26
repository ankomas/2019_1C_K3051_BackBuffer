using BulletSharp;
using BulletSharp.Math;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Movements
{
    public abstract class Movement
    {
        private Vector3 LookAt;
        private float RotationVelocity;
        private float TranslationVelocity;
        
        protected Movement(Vector3 lookAt, float rotationVelocity, float translationVelocity)
        {
            LookAt = lookAt;
            RotationVelocity = rotationVelocity;
            TranslationVelocity = translationVelocity;
        }

        
        
        public MovementToApply Move(TgcMesh mesh, RigidBody rigidBody, TGCVector3 destination)
        {
            var angles = AnglesToRotate(mesh.Position, destination);
            UpdateLookAtVector(angles);
            return new MovementToApply(
                new TGCVector3(angles),
                Matrix.Translation(-LookAt * TranslationVelocity)
            );
        }

        private Vector3 AnglesToRotate(TGCVector3 position, TGCVector3 destination)
        {
            var directions =  DirectionToRotate(position.ToBulletVector3(), destination.ToBulletVector3());
            directions.Normalize();
            return LimitAngles(directions * RotationVelocity);
            
        }

        private void UpdateLookAtVector(Vector3 angles)
        {
            LookAt = Vector3.TransformNormal(LookAt, 
                Matrix.RotationYawPitchRoll(angles.Y,angles.X,angles.Z)
            );
        }


        protected abstract Vector3 DirectionToRotate(Vector3 myPosition, Vector3 destination);
        protected abstract Vector3 LimitAngles(Vector3 anglesToRotate);

    }

    public class MovementToApply
    { 
        public Matrix Translation { get; set; }
        public TGCVector3 AnglesToRotate { get; set; }
        public MovementToApply(TGCVector3 anglesToRotate, Matrix translation)
        {
            AnglesToRotate = anglesToRotate;
            Translation = translation;
        }
    }
}
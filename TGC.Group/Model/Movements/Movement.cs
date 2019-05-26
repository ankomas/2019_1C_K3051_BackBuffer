using BulletSharp;
using BulletSharp.Math;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Movements
{
    public abstract class Movement
    {
        protected TGCVector3 LookAt;
        private readonly float rotationVelocity;
        private readonly float translationVelocity;
        
        protected Movement(TGCVector3 lookAt, float rotationVelocity, float translationVelocity)
        {
            LookAt = lookAt;
            this.rotationVelocity = rotationVelocity;
            this.translationVelocity = translationVelocity;
        }

        
        
        public MovementToApply Move(TGCVector3 position, TGCVector3 destination)
        {
            var angles = AnglesToRotate(position, destination);
            UpdateLookAtVector(angles);
            return new MovementToApply(
                new TGCVector3(angles),
                TGCMatrix.Translation(-LookAt * translationVelocity)
            );
        }

        private TGCVector3 AnglesToRotate(TGCVector3 position, TGCVector3 destination)
        {
            var directions =  DirectionToRotate(position, destination);
            directions.Normalize();
            return LimitAngles(directions * rotationVelocity);
            
        }

        private void UpdateLookAtVector(TGCVector3 angles)
        {
            LookAt = TGCVector3.TransformNormal(LookAt, 
                TGCMatrix.RotationYawPitchRoll(angles.Y,angles.X,angles.Z)
            );
        }


        protected abstract TGCVector3 DirectionToRotate(TGCVector3 myPosition, TGCVector3 destination);
        protected abstract TGCVector3 LimitAngles(TGCVector3 anglesToRotate);

    }

    public class MovementToApply
    { 
        public TGCMatrix Translation { get; set; }
        public TGCVector3 AnglesToRotate { get; set; }
        public MovementToApply(TGCVector3 anglesToRotate, TGCMatrix translation)
        {
            AnglesToRotate = anglesToRotate;
            Translation = translation;
        }
    }
}
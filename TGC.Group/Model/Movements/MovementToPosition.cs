using BulletSharp.Math;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Movements
{
    public class MovementToPosition : Movement
    {
        public MovementToPosition(TGCVector3 lookAt, float rotationVelocity, float translationVelocity): 
            base(lookAt, rotationVelocity, translationVelocity) { }
        

        protected override TGCVector3 LimitAngles(TGCVector3 anglesToRotate)
        {
            return new TGCVector3(0, anglesToRotate.Y, 0);
        }
        
        protected override TGCVector3 DirectionToRotate(TGCVector3 myPosition, TGCVector3 destination)
        { 
            return TGCVector3.Cross( destination - myPosition, LookAt);
        }

    }
}
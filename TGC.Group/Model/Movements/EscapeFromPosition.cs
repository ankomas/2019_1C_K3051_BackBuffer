using TGC.Core.Mathematica;

namespace TGC.Group.Model.Movements
{
    public class EscapeFromPosition : Movement
    {
        public EscapeFromPosition(TGCVector3 lookAt, float rotationVelocity, float translationVelocity) : base(lookAt, rotationVelocity, translationVelocity)
        {
        }

        protected override TGCVector3 DirectionToRotate(TGCVector3 myPosition, TGCVector3 destination)
        {
            return TGCVector3.Cross(  myPosition - destination, LookAt);
        }

        protected override TGCVector3 LimitAngles(TGCVector3 anglesToRotate)
        {
            return new TGCVector3(0f, anglesToRotate.Y, 0f);
        }
    }
}
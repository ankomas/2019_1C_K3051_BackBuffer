using System;
using BulletSharp;
using BulletSharp.Math;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Movements
{
    public class RandomMovement : Movement
    {
        private Random random ;
        public RandomMovement(TGCVector3 lookAt, float rotationVelocity, float translationVelocity) : base(lookAt, rotationVelocity, translationVelocity)
        {
            random = new Random();
        }

        protected override TGCVector3 DirectionToRotate(TGCVector3 myPosition, TGCVector3 destination)
        {
            var dest = new TGCVector3(random.Next(100), 10f, 0f);
            return dest; //TGCVector3.Cross(  dest, LookAt);
        }

        protected override TGCVector3 LimitAngles(TGCVector3 anglesToRotate)
        {
            return new TGCVector3(0f, anglesToRotate.Y, 0f);
        }
    }
}
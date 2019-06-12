using BulletSharp;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Elements.RigidBodyFactories;

namespace TGC.Group.Model
{
    public class CameraFactory
    {
        public static Camera Create(TGCVector3 position, TgcD3dInput input)
        {
            var rigidBody = new CapsuleFactory().Create(position, 100, 60);
            rigidBody.ActivationState = ActivationState.DisableDeactivation;
            return new Camera(position, input, rigidBody);
        }
    }
}
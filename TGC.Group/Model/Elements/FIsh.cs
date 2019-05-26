using BulletSharp;
using Microsoft.DirectX.Direct3D;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;

namespace TGC.Group.Model.Elements
{
    class Fish : Element
    {

        private float ProximityRadius = FastMath.Pow2(200f);
        
        public Fish(TgcMesh model, RigidBody rigidBody) : base(model, rigidBody)
        {
        }

        public override void Update(Camera camera)
        {
            Move(camera);
            base.Update(camera);
            
        }

        private void Move(Camera camera)
        {
            if (IsInTheRange(camera))
            {
                EscapeFromCamera(camera);
            }
            else
            {
                MoveRandomly();
            }
        }

        private void EscapeFromCamera(Camera camera)
        {
            
        }

        private void MoveRandomly()
        {
            
        }

        private bool IsInTheRange(Camera camera)
        {
            return (Mesh.Position - camera.Position).LengthSq() <= ProximityRadius;
        }


        public override IItem item { get; } = new Items.Fish();

    }
}

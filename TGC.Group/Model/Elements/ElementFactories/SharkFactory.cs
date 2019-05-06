using BulletSharp;
using BulletSharp.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Elements.RigidBodyFactories;
using TGC.Group.Model.Resources.Meshes;

namespace TGC.Group.Model.Elements.ElementFactories
{
    class SharkFactory: ElementFactory
    {

        private static SharkFactory instance = null;
        public static SharkFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SharkFactory();
                }
                return instance;
            }
        }

        private SharkFactory() : base(SharkMesh.All(), new CapsuleFactory())
        {
        }
        protected override Element CreateSpecificElement(TgcMesh mesh, RigidBody rigidBody)
        {
            TGCVector3 scaled = new TGCVector3(10, 10, 10);
            rigidBody.CollisionShape.LocalScaling = new Vector3(scaled.X * 3f , scaled.Y, scaled.Z * 1.5f);
            mesh.Scale = scaled;
            return new Shark(mesh, rigidBody);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Resources.Meshes;

namespace TGC.Group.Model.Things
{
    class LifeBelt : Thing
    {
        public LifeBelt() : base(MeshesForShip.LifeBeltMesh)
        {
            SetAmbientShader();
        }


        public override void Render()
        {
            foreach (var mesh in meshes)
            {
                mesh.Render();
                Console.WriteLine("belt: " + mesh.Scale);
            }
        }
    }
}

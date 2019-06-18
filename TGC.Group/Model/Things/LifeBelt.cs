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
        protected override void RenderingPresets()
        {
            base.RenderingPresets();
            ambientShader.SetValue("isBelt", 1);
        }
    }
}

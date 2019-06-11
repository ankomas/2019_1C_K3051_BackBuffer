using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Group.Model.Resources.Meshes;

namespace TGC.Group.Model.Things
{
    class Hatch : Thing
    {
        Effect shader;
        public Hatch(Callback callback) : base(MeshesForShip.HatchMesh, "Hatch", "Exit ship", callback)
        {
            SetAmbientShader();
        }
    }
}

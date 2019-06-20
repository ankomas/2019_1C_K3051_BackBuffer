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
    class Ship : Thing
    {
        public Ship() : base(MeshesForShip.ShipMesh)
        {
            SetAmbientShader();
        }
        protected override void RenderingPresets()
        {
            base.RenderingPresets();
            ambientShader.SetValue("normalDirection", -1f);
        }
    }
}

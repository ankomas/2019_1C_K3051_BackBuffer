using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Resources.Meshes;
using static TGC.Group.Model.Scenes.ShipScene;

namespace TGC.Group.Model.Things
{
    class Seat : Thing
    {
        public Seat() : base(MeshesForShip.SeatMesh)
        {
            SetAmbientShader();
        }
    }
}

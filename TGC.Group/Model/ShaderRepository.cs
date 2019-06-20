using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;

namespace TGC.Group.Model
{
    class ShaderRepository
    {
        public static Effect ShipAmbientShader = Load("ShipAmbient.fx");
        public static Effect LaserShader = Load("Laser.fx");
        private static Effect Load(string path)
        {
            Effect effect;
            string e = "";
            try
            {
                effect = Effect.FromFile(D3DDevice.Instance.Device, Game.Default.ShadersDirectory + path, null, null, ShaderFlags.None, null, out e);
            }
            catch (Exception)
            {
                throw new Exception("CTM" + e);
            }
            if (effect == null)
            {
                throw new Exception("CTM carrajo" + e);
            }
            return effect;
        }
    }
}

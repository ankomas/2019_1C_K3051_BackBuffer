using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;

namespace TGC.Group.Model
{
    class ShaderRepository
    {
        public static Effect ShipAmbientShader = Load("ShipAmbient");
        public static Effect LaserShader = Load("Laser");
        public static Effect DialogBoxShader = Load("DialogBox"); 
        public static Effect WorldWaterFog = Load("WorldWaterFog");
        public static Effect FedeSkybox = Load("FedeSkybox");
        public static Effect Plane = Load("Plane");
        private static Effect Load(string path)
        {
            Effect effect;
            string e = "";
            try
            {
                effect = Effect.FromFile(D3DDevice.Instance.Device, Game.Default.ShadersDirectory + path + ".fx", null, null, ShaderFlags.None, null, out e);
            }
            catch (Exception)
            {
                throw new Exception("Failed effect file loading");
            }
            if (effect == null)
            {
                throw new Exception("Failed shader compiling: " + e);
            }
            return effect;
        }
        public static void DeleteShaderFromTgcMesh(TgcMesh tgcMesh)
        {
            tgcMesh.Effect = TGCShaders.Instance.TgcMeshShader;
            tgcMesh.Technique = TGCShaders.Instance.GetTGCMeshTechnique(tgcMesh.RenderType);
        }
    }
}

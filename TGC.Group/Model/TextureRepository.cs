using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;

namespace TGC.Group.Model
{
    class TextureRepository
    {
        public static Texture SkyboxOutside = TextureLoader.FromFile(D3DDevice.Instance.Device, Game.Default.MediaDirectory + "lol6.jpg");
    }
}

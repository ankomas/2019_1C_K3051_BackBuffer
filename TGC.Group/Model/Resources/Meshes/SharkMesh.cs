using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Resources.Meshes
{
    static class SharkMesh
    {
        private static TgcMesh Shark = new TgcSceneLoader()
                        .loadSceneFromFile(Game.Default.MediaDirectory + "shark-TgcScene.xml").Meshes[0];

        public static TgcMesh Get()
        {
            if (Shark == null || Shark.D3dMesh == null)
            {
                Shark = new TgcSceneLoader()
                    .loadSceneFromFile(Game.Default.MediaDirectory + "shark-TgcScene.xml").Meshes[0];
            }
            return Shark;
        }
    }
}

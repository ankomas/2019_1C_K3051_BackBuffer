using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Resources.Meshes
{
    class MeshesForShip
    {
        private static TgcMesh CrafterBodyMesh = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "crafter-body-TgcScene.xml").Meshes[0];
        private static TgcMesh CrafterFrontCoverMesh = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "crafter-front-cover-TgcScene.xml").Meshes[0];
        private static TgcMesh CrafterTopCoverMesh = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "crafter-top-cover-TgcScene.xml").Meshes[0];

        public static List<TgcMesh> CrafterMeshes = new List<TgcMesh>(new TgcMesh[] { CrafterBodyMesh, CrafterFrontCoverMesh, CrafterTopCoverMesh });
        public static List<TgcMesh> HatchMesh     = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "hatch-TgcScene.xml").Meshes;
        public static List<TgcMesh> ShipMesh      = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "new-ship-2-TgcScene.xml").Meshes;
        public static List<TgcMesh> SeatMesh      = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "seat-TgcScene.xml").Meshes;
        public static List<TgcMesh> LifeBeltMesh  = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "life-belt-TgcScene.xml").Meshes;
        public static List<TgcMesh> O2Mesh = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "o2Tank-TgcScene.xml").Meshes;
    }
}
    
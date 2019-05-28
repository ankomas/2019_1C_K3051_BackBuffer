using System;
using System.Collections.Generic;
using System.Linq;
using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Elements;
using TGC.Group.Model.Elements.RigidBodyFactories;
using Element = TGC.Group.Model.Elements.Element;

namespace TGC.Group.Model.Chunks
{
    public class InitialChunk : Chunk
    {
        public InitialChunk(TGCVector3 origin) : base(origin, AquaticPhysics.Instance)
        {

        }

        public override IEnumerable<Element> Init()
        {
            var scene = new TgcSceneLoader().loadSceneFromFile(Game.Default.MediaDirectory + "ship-TgcScene.xml");
            scene.Meshes.ForEach(mesh => mesh.Scale = mesh.Scale*5);

            var sceneBoundingBox = scene.BoundingBox;

            var boundingBox = new TgcBoundingAxisAlignBox(sceneBoundingBox.PMin, sceneBoundingBox.PMax*5);
            
            var ship = new Ship(scene, new BoxFactory().Create(boundingBox));
            
            AquaticPhysics.Instance.Add(ship.PhysicsBody);

            return new List<Element> {ship};
        }
    }
    
    
}